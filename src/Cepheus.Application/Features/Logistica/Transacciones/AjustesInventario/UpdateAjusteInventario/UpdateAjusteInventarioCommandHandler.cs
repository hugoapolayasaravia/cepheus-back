using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.UpdateAjusteInventario;

public sealed class UpdateAjusteInventarioCommandHandler
    : IRequestHandler<UpdateAjusteInventarioCommand, AjusteInventarioResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateAjusteInventarioCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<AjusteInventarioResponse> Handle(
        UpdateAjusteInventarioCommand request,
        CancellationToken ct)
    {
        var planta = AjusteInventarioRules.Normalize(request.PlantaCode);
        var code = AjusteInventarioRules.Normalize(request.Code);

        var ajuste = await AjusteInventarioReader.LoadForUpdateAsync(_uow, planta, code, ct);

        AjusteInventarioRules.EnsureLineasAgregables(ajuste);

        ajuste.FechaEntrega = await AjusteInventarioRules.ValidateFechaEntregaAsync(
            _uow,
            planta,
            request.FechaEntrega,
            ct);

        ajuste.Observacion = request.Observacion?.Trim();

        // El IGV depende de la fecha de entrega: se recalculan los importes.
        await AjusteInventarioRules.RecalculateTotalsAsync(_uow, ajuste, ct);

        _uow.Logistica.Transacciones.AjustesInventario.Update(ajuste);
        await AjusteInventarioReader.SaveAsync(_uow, ct);

        return await AjusteInventarioReader.GetAsync(_uow, planta, code, ct);
    }
}
