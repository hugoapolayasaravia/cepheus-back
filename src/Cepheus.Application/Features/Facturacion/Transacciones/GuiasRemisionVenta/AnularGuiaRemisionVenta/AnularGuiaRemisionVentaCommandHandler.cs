using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.AnularGuiaRemisionVenta;

public sealed class AnularGuiaRemisionVentaCommandHandler
    : IRequestHandler<AnularGuiaRemisionVentaCommand, GuiaRemisionVentaResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public AnularGuiaRemisionVentaCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<GuiaRemisionVentaResponse> Handle(AnularGuiaRemisionVentaCommand request, CancellationToken ct)
    {
        var number = GuiaRemisionVentaRules.NormalizeNumber(request.Number);
        var g = await GuiaRemisionVentaReader.LoadForUpdateAsync(_uow, number, ct);

        // PB: solo Estado '01'. Una guía facturada o cancelada se libera primero anulando su comprobante (entregable 5).
        if (g.Status != EstadoGuiaRemisionVenta.Pendiente)
        {
            throw new InvalidOperationException($"La guía {number} está {g.Status}; solo se anula una guía Pendiente.");
        }

        await GuiaRemisionVentaRules.EnsurePeriodOpenAsync(_uow, g.IssueDate, ct);

        g.Status = EstadoGuiaRemisionVenta.Anulado;
        g.CanceledAt = DateTime.Now;
        g.CanceledBy = _currentUser.FullName;
        g.CancelReason = OrdenTrabajoVentaRules.NormalizeText(request.Data.Reason);

        if (g.OrdenTrabajoCode is not null)
        {
            var ot = await GuiaRemisionVentaRules.LoadOrderAsync(_uow, g.OrdenTrabajoCode, ct);
            await GuiaRemisionVentaSaldos.RecalcAsync(_uow, ot, g, ct);
        }

        await GuiaRemisionVentaRules.SaveAsync(_uow, "la anulación de la guía", ct);

        return await GuiaRemisionVentaReader.GetAsync(_uow, number, ct);
    }
}
