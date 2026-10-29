using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.AnularHojaCorte;

public sealed class AnularHojaCorteCommandHandler : IRequestHandler<AnularHojaCorteCommand, HojaCorteResponse>
{
    private readonly IUnitOfWork _uow;

    public AnularHojaCorteCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<HojaCorteResponse> Handle(AnularHojaCorteCommand request, CancellationToken ct)
    {
        var year = HojaCorteRules.Normalize(request.Year);
        var code = HojaCorteRules.NormalizeCode(request.Code);

        var hc = await HojaCorteReader.LoadForUpdateAsync(_uow, year, code, ct);

        if (hc.Status == EstadoHojaCorte.Anulado)
        {
            throw new InvalidOperationException("La Hoja de Corte ya está anulada.");
        }

        if (hc.Status != EstadoHojaCorte.Pendiente)
        {
            throw new InvalidOperationException(
                $"La Hoja de Corte se encuentra {hc.Status}; solo se anula en estado Pendiente.");
        }

        if (!string.IsNullOrWhiteSpace(hc.OrdenTrabajoCode))
        {
            throw new InvalidOperationException(
                $"La Hoja de Corte tiene asociada la Orden de Trabajo {hc.OrdenTrabajoCode}; no se puede anular.");
        }

        hc.Status = EstadoHojaCorte.Anulado;

        await HojaCorteReader.SaveAsync(_uow, ct);

        return await HojaCorteReader.GetAsync(_uow, year, code, ct);
    }
}
