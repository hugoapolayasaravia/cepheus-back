using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.CreateVale;

/// <summary>
/// Cabecera, líneas y materiales de la OT asignados se confirman con UN solo SaveChanges
/// (una sola transacción): si algo falla no queda nada a medias.
/// </summary>
public sealed class CreateValeCommandHandler
    : IRequestHandler<CreateValeCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateValeCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ValeResponse> Handle(
        CreateValeCommand request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var tipoVale = ValeRules.Normalize(request.TipoValeCode);
        var otCode = ValeRules.NormalizeOrNull(request.OrdenTrabajoCode);

        OrdenTrabajo? ordenTrabajo = null;

        if (otCode is not null)
        {
            ordenTrabajo = await ValeCabecera.LoadOrdenTrabajoEnEjecucionAsync(
                _uow,
                planta,
                otCode,
                ct);
        }

        var cabecera = await ValeCabecera.ResolverAsync(
            _uow,
            planta,
            tipoVale,
            request.FechaEntrega,
            request.SubCentroCostoCode,
            request.SubCentroEjecutorCode,
            request.TrabajadorCode,
            request.UnidadNegocioCode,
            request.PlantaAfectadaCode,
            otCode,
            ordenTrabajo,
            ct);

        var repetidos = request.Detalles
            .Select(d => ValeRules.Normalize(d.ArticuloCode))
            .Concat(request.MaterialesOrdenTrabajo.Select(m => ValeRules.Normalize(m.ArticuloCode)))
            .GroupBy(a => a)
            .Any(g => g.Count() > 1);

        if (repetidos)
        {
            throw new InvalidOperationException(
                "Un mismo artículo no puede repetirse en el Vale de Salida.");
        }

        if (request.MaterialesOrdenTrabajo.Count > 0 && otCode is null)
        {
            throw new InvalidOperationException(
                "Solo se pueden asignar materiales de una Orden de Trabajo si el vale tiene OT.");
        }

        var vale = new Vale
        {
            PlantaCode = planta,
            TipoValeCode = cabecera.TipoValeCode,
            FechaEntrega = cabecera.FechaEntrega,
            SubCentroCostoCode = cabecera.SubCentroCostoCode,
            SubCentroEjecutorCode = cabecera.SubCentroEjecutorCode,
            TrabajadorCode = cabecera.TrabajadorCode,
            OrdenTrabajoCode = otCode,
            UnidadNegocioCode = cabecera.UnidadNegocioCode,
            PlantaAfectadaCode = cabecera.PlantaAfectadaCode,
            Estado = EstadoVale.Pendiente
        };

        // Líneas manuales.
        foreach (var linea in request.Detalles)
        {
            var articulo = ValeRules.Normalize(linea.ArticuloCode);

            var resuelta = await ValeLineas.ResolverAsync(
                _uow,
                planta,
                cabecera.TipoValeCode,
                articulo,
                linea.Cantidad,
                linea.Propiedad01,
                0m,
                null,
                true,
                ct);

            vale.Detalles.Add(new ValeDetalle
            {
                PlantaCode = planta,
                ArticuloCode = articulo,
                Cantidad = linea.Cantidad,
                Precio = resuelta.Precio,
                Total = ValeRules.LineTotal(linea.Cantidad, resuelta.Precio),
                Estado = EstadoValeDetalle.Pendiente,
                Propiedad01 = resuelta.Propiedad01
            });
        }

        // Materiales pendientes de la OT: pasan a "asignado a vale" (15).
        foreach (var solicitado in request.MaterialesOrdenTrabajo)
        {
            var articulo = ValeRules.Normalize(solicitado.ArticuloCode);

            var material = await _uow.Mantenimiento.Transacciones.OTRMateriales
                .Query()
                .FirstOrDefaultAsync(
                    m => m.PlantaCode == planta &&
                         m.OrdenTrabajoCode == otCode &&
                         m.FechaProceso == solicitado.FechaProceso &&
                         m.ArticuloCode == articulo,
                    ct);

            if (material is null ||
                material.EstadoCode != ValeRules.OtMaterialPendiente)
            {
                throw new InvalidOperationException(
                    $"El material {articulo} de la Orden de Trabajo {otCode} no existe o ya no está pendiente.");
            }

            var resuelta = await ValeLineas.ResolverAsync(
                _uow,
                planta,
                cabecera.TipoValeCode,
                articulo,
                material.Cantidad,
                null,
                0m,
                material.CostoUnitario > 0 ? material.CostoUnitario : null,
                false,
                ct);

            material.EstadoCode = ValeRules.OtMaterialAsignado;

            vale.Detalles.Add(new ValeDetalle
            {
                PlantaCode = planta,
                ArticuloCode = articulo,
                Cantidad = material.Cantidad,
                Precio = resuelta.Precio,
                Total = ValeRules.LineTotal(material.Cantidad, resuelta.Precio),
                Estado = EstadoValeDetalle.Pendiente,
                Propiedad01 = resuelta.Propiedad01,
                MaterialOtFechaProceso = material.FechaProceso
            });
        }

        ValeLineas.Renumerar(vale);
        await ValeRules.RecalculateTotalsAsync(_uow, vale, ct);

        vale.Code = await ValeRules.NextCodeAsync(_uow, planta, ct);

        foreach (var detalle in vale.Detalles)
        {
            detalle.ValeCode = vale.Code;
        }

        await _uow.Logistica.Transacciones.Vales.AddAsync(vale, ct);
        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, vale.Code, ct);
    }
}
