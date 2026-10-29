using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.PasarAProduccionOrdenTrabajoVenta;

public sealed class PasarAProduccionOrdenTrabajoVentaCommandHandler
    : IRequestHandler<PasarAProduccionOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public PasarAProduccionOrdenTrabajoVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(PasarAProduccionOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);
        OrdenTrabajoVentaRules.EnsureActive(ot, "pase a producción");

        if (ot.Stage != EtapaOrdenTrabajoVenta.AprobadaCredito)
        {
            throw new InvalidOperationException(
                $"La Orden de Trabajo está {ot.Stage}; el pase a producción es para una OT aprobada por Créditos.");
        }

        if (ot.PassedToProduction)
        {
            throw new InvalidOperationException("La Orden de Trabajo ya pasó a producción.");
        }

        ot.PassedToProduction = true;
        ot.ProductionPassDate = DateTime.Now;

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
