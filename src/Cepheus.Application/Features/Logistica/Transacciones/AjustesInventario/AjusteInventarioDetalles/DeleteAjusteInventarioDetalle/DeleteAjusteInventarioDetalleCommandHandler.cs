using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AjusteInventarioDetalles.DeleteAjusteInventarioDetalle;

public sealed class DeleteAjusteInventarioDetalleCommandHandler
    : IRequestHandler<DeleteAjusteInventarioDetalleCommand, AjusteInventarioResponse>
{
    private readonly IUnitOfWork _uow;

    public DeleteAjusteInventarioDetalleCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<AjusteInventarioResponse> Handle(
        DeleteAjusteInventarioDetalleCommand request,
        CancellationToken ct)
    {
        var planta = AjusteInventarioRules.Normalize(request.PlantaCode);
        var code = AjusteInventarioRules.Normalize(request.AjusteCode);
        var articulo = AjusteInventarioRules.Normalize(request.ArticuloCode);

        var ajuste = await AjusteInventarioReader.LoadForUpdateAsync(_uow, planta, code, ct);

        AjusteInventarioRules.EnsureLineasAgregables(ajuste);

        var detalle = ajuste.Detalles.FirstOrDefault(d => d.ArticuloCode == articulo)
            ?? throw new KeyNotFoundException(
                $"El artículo {articulo} no está en el Ajuste de Inventario {planta}/{code}.");

        if (detalle.Estado != EstadoAjusteInventarioDetalle.Pendiente)
        {
            throw new InvalidOperationException(
                $"La línea del artículo {articulo} está {detalle.Estado} y no se puede eliminar.");
        }

        ajuste.Detalles.Remove(detalle);
        _uow.Logistica.Transacciones.AjusteInventarioDetalles.Remove(detalle);

        AjusteInventarioLineas.Renumerar(ajuste);
        await AjusteInventarioRules.RecalculateTotalsAsync(_uow, ajuste, ct);

        await AjusteInventarioReader.SaveAsync(_uow, ct);

        return await AjusteInventarioReader.GetAsync(_uow, planta, code, ct);
    }
}
