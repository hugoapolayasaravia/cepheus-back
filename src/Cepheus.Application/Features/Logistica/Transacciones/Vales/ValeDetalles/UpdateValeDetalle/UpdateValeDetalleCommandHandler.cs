using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ValeDetalles.UpdateValeDetalle;

public sealed class UpdateValeDetalleCommandHandler
    : IRequestHandler<UpdateValeDetalleCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateValeDetalleCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ValeResponse> Handle(
        UpdateValeDetalleCommand request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var code = ValeRules.Normalize(request.ValeCode);
        var articulo = ValeRules.Normalize(request.ArticuloCode);

        var vale = await ValeReader.LoadForUpdateAsync(_uow, planta, code, ct);

        ValeRules.EnsureEditable(vale, "modificar líneas");

        var detalle = vale.Detalles.FirstOrDefault(d => d.ArticuloCode == articulo)
            ?? throw new KeyNotFoundException(
                $"El artículo {articulo} no está en el Vale de Salida {planta}/{code}.");

        if (detalle.Estado != EstadoValeDetalle.Pendiente)
        {
            throw new InvalidOperationException(
                $"La línea del artículo {articulo} está {detalle.Estado} y no se puede modificar.");
        }

        var resuelta = await ValeLineas.ResolverAsync(
            _uow,
            planta,
            vale.TipoValeCode,
            articulo,
            request.Cantidad,
            request.Propiedad01,
            detalle.Cantidad,
            detalle.Precio,
            true,
            ct);

        detalle.Cantidad = request.Cantidad;
        detalle.Total = ValeRules.LineTotal(request.Cantidad, detalle.Precio);
        detalle.Propiedad01 = resuelta.Propiedad01;

        await ValeRules.RecalculateTotalsAsync(_uow, vale, ct);

        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, code, ct);
    }
}
