using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetCreditoOrdenTrabajoVenta;

public sealed class GetCreditoOrdenTrabajoVentaQueryHandler
    : IRequestHandler<GetCreditoOrdenTrabajoVentaQuery, CreditoOrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public GetCreditoOrdenTrabajoVentaQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<CreditoOrdenTrabajoVentaResponse> Handle(GetCreditoOrdenTrabajoVentaQuery request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await _uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(o => o.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Orden de Trabajo {code} no existe.");

        var (cliente, obra, _) = await OrdenTrabajoVentaAprobaciones.GetPaymentContextAsync(_uow, ot, ct);
        var credito = await OrdenTrabajoVentaCredito.GetAsync(_uow, cliente, obra, ct);

        return new CreditoOrdenTrabajoVentaResponse
        {
            OrdenTrabajoCode = ot.Code,
            ClienteCode = ot.ClienteCode,
            ObraCode = ot.ObraCode,
            OrderTotal = ot.Total,
            Mode = credito.Mode,
            Limit = credito.Limit,
            Used = credito.Used,
            Available = credito.Available,
            Required = OrdenTrabajoVentaCredito.IsRequired(ot, cliente, obra),
            Covers = ot.Total <= credito.Available,
            Note = credito.Note
        };
    }
}
