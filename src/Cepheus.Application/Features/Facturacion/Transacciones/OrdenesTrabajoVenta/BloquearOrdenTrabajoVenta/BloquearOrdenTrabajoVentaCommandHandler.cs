using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.BloquearOrdenTrabajoVenta;

/// <summary>
/// Envía la OT a aprobación (PB: ges_sp_AgregaOrdenTrabajoAprueba). Se aprueba sola si es Normal y el cliente es VIP o
/// no requiere aprobación de OT, o si es Descuento/Saldo (copian la aprobación de su origen) y la obra es a crédito.
/// En los demás casos queda bloqueada y Pendiente para Créditos.
/// </summary>
public sealed class BloquearOrdenTrabajoVentaCommandHandler
    : IRequestHandler<BloquearOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public BloquearOrdenTrabajoVentaCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<OrdenTrabajoVentaResponse> Handle(BloquearOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);
        OrdenTrabajoVentaRules.EnsureEditable(ot);
        OrdenTrabajoVentaAprobaciones.EnsureHasLines(ot);

        var (cliente, obra, isCredit) = await OrdenTrabajoVentaAprobaciones.GetPaymentContextAsync(_uow, ot, ct);

        // PB (ot_ventas, bloqueo): cliente VIP o que no requiere aprobación de OT => si la obra es al contado no se puede
        // aprobar automáticamente (coordinar con Créditos); si es a crédito pasa solo. Cliente que sí requiere aprobación:
        // solo la OT Descuento pasa sola; las demás quedan bloqueadas para Créditos.
        var manual = OrdenTrabajoVentaAprobaciones.RequiresManualApproval(cliente);
        bool auto;

        if (!manual)
        {
            if (!isCredit)
            {
                throw new InvalidOperationException(
                    "La forma de pago de la obra es al contado: no se puede aprobar automáticamente. Coordine con Créditos.");
            }

            auto = OrdenTrabajoVentaAprobaciones.IsAutoApprovableType(ot.Type);
        }
        else
        {
            auto = ot.Type == TipoOrdenTrabajoVenta.Descuento;
        }

        ot.IsModifiable = false;
        ot.SentToApprovalAt = DateTime.Now;

        if (auto)
        {
            var approval = await OrdenTrabajoVentaAprobaciones.BuildAutoAsync(_uow, ot, cliente, obra, ct);

            OrdenTrabajoVentaAprobaciones.ApplyToOrder(
                ot,
                approval,
                _currentUser.FullName,
                OrdenTrabajoVentaAprobaciones.AutoCreditsObservations,
                passToProduction: true);
        }

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
