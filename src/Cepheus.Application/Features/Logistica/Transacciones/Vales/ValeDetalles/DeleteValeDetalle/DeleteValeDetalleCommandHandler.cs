using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ValeDetalles.DeleteValeDetalle;

public sealed class DeleteValeDetalleCommandHandler
    : IRequestHandler<DeleteValeDetalleCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;

    public DeleteValeDetalleCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ValeResponse> Handle(
        DeleteValeDetalleCommand request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var code = ValeRules.Normalize(request.ValeCode);
        var articulo = ValeRules.Normalize(request.ArticuloCode);

        var vale = await ValeReader.LoadForUpdateAsync(_uow, planta, code, ct);

        ValeRules.EnsureEditable(vale, "eliminar líneas");

        var detalle = vale.Detalles.FirstOrDefault(d => d.ArticuloCode == articulo)
            ?? throw new KeyNotFoundException(
                $"El artículo {articulo} no está en el Vale de Salida {planta}/{code}.");

        await ValeOrdenTrabajo.ReleaseAsync(_uow, vale, detalle, ct);

        vale.Detalles.Remove(detalle);
        _uow.Logistica.Transacciones.ValeDetalles.Remove(detalle);

        ValeLineas.Renumerar(vale);
        await ValeRules.RecalculateTotalsAsync(_uow, vale, ct);

        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, code, ct);
    }
}
