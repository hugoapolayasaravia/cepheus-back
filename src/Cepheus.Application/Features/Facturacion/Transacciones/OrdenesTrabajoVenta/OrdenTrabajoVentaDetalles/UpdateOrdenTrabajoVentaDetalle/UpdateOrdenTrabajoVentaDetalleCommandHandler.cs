using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.UpdateOrdenTrabajoVentaDetalle;

public sealed class UpdateOrdenTrabajoVentaDetalleCommandHandler
    : IRequestHandler<UpdateOrdenTrabajoVentaDetalleCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateOrdenTrabajoVentaDetalleCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(UpdateOrdenTrabajoVentaDetalleCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);
        OrdenTrabajoVentaRules.EnsureEditable(ot);

        var ctx = await OrdenTrabajoVentaLineas.BuildContextAsync(_uow, ot, ct);
        var line = OrdenTrabajoVentaLineas.FindLine(ot, request.Item);

        if (line.Status == EstadoDetalleOrdenTrabajoVenta.Anulado)
        {
            throw new InvalidOperationException("La línea está anulada, imposible su modificación.");
        }

        OrdenTrabajoVentaRules.EnsureLineEditable(ot, line, ctx.HasHojasCorte);
        OrdenTrabajoVentaLineas.EnsureAmounts(request.Data.Quantity, request.Data.UnitPrice);

        var price = request.Data.UnitPrice;

        if (ctx.Cotizacion is not null && line.NegocioCode == ctx.Cotizacion.Cotizacion.NegocioCode)
        {
            // Devuelve lo anterior y consume lo nuevo; el precio no puede superar el de la cotización.
            ctx.Cotizacion.Return(line.ProductoTipoCode, line.ProductoCode, line.Quantity);
            var cotPrice = ctx.Cotizacion.Consume(line.ProductoTipoCode, line.ProductoCode, request.Data.Quantity);
            OrdenTrabajoVentaLineas.EnsurePriceNotAbove(price, cotPrice, line.ProductoTipoCode, line.ProductoCode);
        }

        var signed = OrdenTrabajoVentaRules.SignedPrice(line.ProductoTipoCode, price);

        line.Quantity = request.Data.Quantity;
        line.UnitPrice = signed;
        line.Total = OrdenTrabajoVentaRules.LineTotal(line.Quantity, signed);

        OrdenTrabajoVentaRules.Recalculate(ot);
        await OrdenTrabajoVentaLineas.EnsureAnticipoBalanceAsync(_uow, ot, ct);
        await OrdenTrabajoVentaLineas.EnsureCreditAsync(_uow, ot, ct);

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
