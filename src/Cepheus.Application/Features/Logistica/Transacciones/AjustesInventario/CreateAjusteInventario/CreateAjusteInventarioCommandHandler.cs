using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.CreateAjusteInventario;

/// <summary>Cabecera y líneas se confirman con UN solo SaveChanges (una sola transacción).</summary>
public sealed class CreateAjusteInventarioCommandHandler
    : IRequestHandler<CreateAjusteInventarioCommand, AjusteInventarioResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateAjusteInventarioCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<AjusteInventarioResponse> Handle(
        CreateAjusteInventarioCommand request,
        CancellationToken ct)
    {
        var planta = AjusteInventarioRules.Normalize(request.PlantaCode);

        var plantaExiste = await _uow.Comunes.Plantas
            .Query()
            .AsNoTracking()
            .AnyAsync(p => p.Code == planta, ct);

        if (!plantaExiste)
        {
            throw new InvalidOperationException($"La planta {planta} no existe.");
        }

        var entrega = await AjusteInventarioRules.ValidateFechaEntregaAsync(
            _uow,
            planta,
            request.FechaEntrega,
            ct);

        var repetidos = request.Detalles
            .GroupBy(d => AjusteInventarioRules.Normalize(d.ArticuloCode))
            .Any(g => g.Count() > 1);

        if (repetidos)
        {
            throw new InvalidOperationException(
                "Un mismo artículo no puede repetirse en el Ajuste de Inventario.");
        }

        var ajuste = new AjusteInventario
        {
            PlantaCode = planta,
            Observacion = request.Observacion?.Trim(),
            FechaEntrega = entrega,
            Estado = EstadoAjusteInventario.Pendiente
        };

        foreach (var linea in request.Detalles)
        {
            var articulo = AjusteInventarioRules.Normalize(linea.ArticuloCode);

            var resuelta = await AjusteInventarioLineas.ResolverAsync(
                _uow,
                planta,
                articulo,
                linea.Cantidad,
                null,
                ct);

            ajuste.Detalles.Add(new AjusteInventarioDetalle
            {
                PlantaCode = planta,
                ArticuloCode = articulo,
                Tipo = linea.Tipo,
                Cantidad = linea.Cantidad,
                Precio = resuelta.Precio,
                Total = AjusteInventarioRules.LineTotal(linea.Cantidad, resuelta.Precio),
                Estado = EstadoAjusteInventarioDetalle.Pendiente
            });
        }

        AjusteInventarioLineas.Renumerar(ajuste);
        await AjusteInventarioRules.RecalculateTotalsAsync(_uow, ajuste, ct);

        ajuste.Code = await AjusteInventarioRules.NextCodeAsync(_uow, planta, ct);

        foreach (var detalle in ajuste.Detalles)
        {
            detalle.AjusteCode = ajuste.Code;
        }

        await _uow.Logistica.Transacciones.AjustesInventario.AddAsync(ajuste, ct);
        await AjusteInventarioReader.SaveAsync(_uow, ct);

        return await AjusteInventarioReader.GetAsync(_uow, planta, ajuste.Code, ct);
    }
}
