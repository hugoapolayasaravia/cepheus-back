using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ValeDetalles.CreateValeDetalle;

public sealed class CreateValeDetalleCommandHandler
    : IRequestHandler<CreateValeDetalleCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateValeDetalleCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ValeResponse> Handle(
        CreateValeDetalleCommand request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var code = ValeRules.Normalize(request.ValeCode);
        var articulo = ValeRules.Normalize(request.ArticuloCode);

        var vale = await ValeReader.LoadForUpdateAsync(_uow, planta, code, ct);

        ValeRules.EnsureEditable(vale, "agregar líneas");

        if (vale.Detalles.Any(d => d.ArticuloCode == articulo))
        {
            throw new InvalidOperationException(
                $"El artículo {articulo} ya está registrado en el Vale de Salida.");
        }

        var resuelta = await ValeLineas.ResolverAsync(
            _uow,
            planta,
            vale.TipoValeCode,
            articulo,
            request.Cantidad,
            request.Propiedad01,
            0m,
            null,
            true,
            ct);

        var detalle = new ValeDetalle
        {
            PlantaCode = planta,
            ValeCode = code,
            ArticuloCode = articulo,
            Cantidad = request.Cantidad,
            Precio = resuelta.Precio,
            Total = ValeRules.LineTotal(request.Cantidad, resuelta.Precio),
            Estado = EstadoValeDetalle.Pendiente,
            Propiedad01 = resuelta.Propiedad01
        };

        vale.Detalles.Add(detalle);

        ValeLineas.Renumerar(vale);
        await ValeRules.RecalculateTotalsAsync(_uow, vale, ct);

        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, code, ct);
    }
}
