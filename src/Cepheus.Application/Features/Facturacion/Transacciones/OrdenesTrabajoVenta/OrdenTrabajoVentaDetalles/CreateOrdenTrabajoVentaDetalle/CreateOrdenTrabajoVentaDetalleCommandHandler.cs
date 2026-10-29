using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.CreateOrdenTrabajoVentaDetalle;

public sealed class CreateOrdenTrabajoVentaDetalleCommandHandler
    : IRequestHandler<CreateOrdenTrabajoVentaDetalleCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateOrdenTrabajoVentaDetalleCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(CreateOrdenTrabajoVentaDetalleCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);
        OrdenTrabajoVentaRules.EnsureEditable(ot);

        var ctx = await OrdenTrabajoVentaLineas.BuildContextAsync(_uow, ot, ct);

        await OrdenTrabajoVentaLineas.AddAsync(_uow, ctx, request.Data, ct);

        OrdenTrabajoVentaRules.Recalculate(ot);
        await OrdenTrabajoVentaLineas.EnsureAnticipoBalanceAsync(_uow, ot, ct);
        await OrdenTrabajoVentaLineas.EnsureCreditAsync(_uow, ot, ct);

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
