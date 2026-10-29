using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.DeleteOrdenTrabajoVentaDetalle;

public sealed class DeleteOrdenTrabajoVentaDetalleCommandHandler
    : IRequestHandler<DeleteOrdenTrabajoVentaDetalleCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public DeleteOrdenTrabajoVentaDetalleCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(DeleteOrdenTrabajoVentaDetalleCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);
        OrdenTrabajoVentaRules.EnsureEditable(ot);

        var ctx = await OrdenTrabajoVentaLineas.BuildContextAsync(_uow, ot, ct);
        var line = OrdenTrabajoVentaLineas.FindLine(ot, request.Item);

        // PB: las líneas de una OT que proviene de cotización SÍ se pueden quitar (devuelven saldo);
        // las de Hoja de Corte solo se liberan anulando la OT.
        if (ctx.HasHojasCorte && OrdenTrabajoVentaRules.IsHojaCorteProducto(line.ProductoTipoCode))
        {
            throw new InvalidOperationException(
                "Imposible eliminar: la línea proviene de una Hoja de Corte. Para cambiarla anule la OT (libera las Hojas de Corte).");
        }

        if (ctx.Cotizacion is not null && line.NegocioCode == ctx.Cotizacion.Cotizacion.NegocioCode)
        {
            ctx.Cotizacion.Return(line.ProductoTipoCode, line.ProductoCode, line.Quantity);
        }

        ot.Detalles.Remove(line);
        _uow.Facturacion.Transacciones.OrdenesTrabajoVentaDetalle.Remove(line);

        OrdenTrabajoVentaRules.Recalculate(ot);

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
