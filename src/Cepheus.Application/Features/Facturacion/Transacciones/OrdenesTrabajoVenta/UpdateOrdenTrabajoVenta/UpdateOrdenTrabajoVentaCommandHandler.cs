using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.UpdateOrdenTrabajoVenta;

public sealed class UpdateOrdenTrabajoVentaCommandHandler
    : IRequestHandler<UpdateOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateOrdenTrabajoVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(UpdateOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);
        OrdenTrabajoVentaRules.EnsureEditable(ot);

        var obra = await OrdenTrabajoVentaCabecera.GetObraAsync(_uow, ot.ClienteCode, ot.ObraCode, ct);

        var cot = await OrdenTrabajoVentaCotizacion.LoadAsync(_uow, ot, ct);

        await OrdenTrabajoVentaCabecera.ApplyAsync(_uow, ot, request.Data, obra, cot?.Cotizacion.CurrencyCode, ct);

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
