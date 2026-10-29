using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.UpdateHojaCorte;

public sealed class UpdateHojaCorteCommandHandler : IRequestHandler<UpdateHojaCorteCommand, HojaCorteResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateHojaCorteCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<HojaCorteResponse> Handle(UpdateHojaCorteCommand request, CancellationToken ct)
    {
        var year = HojaCorteRules.Normalize(request.Year);
        var code = HojaCorteRules.NormalizeCode(request.Code);

        var hc = await HojaCorteReader.LoadForUpdateAsync(_uow, year, code, ct);
        HojaCorteRules.EnsureEditable(hc);

        var data = request.Data;

        await HojaCorteCabecera.ApplyAsync(_uow, hc, data, ct);

        // El padre no cambia, pero el nuevo tipo debe seguir siendo coherente con él.
        var requiresParent = Cepheus.Domain.Facturacion.Catalogos.TipoHojaCorteCodes.RequiresParent(hc.TipoHojaCorteCode);
        if (requiresParent && hc.ParentCode is null)
        {
            throw new InvalidOperationException(
                "El tipo de Hoja de Corte indicado requiere una Hoja de Corte padre y esta hoja no la tiene.");
        }

        foreach (var linea in hc.Detalles)
        {
            HojaCorteLineas.EnsureConsistent(hc, linea.ProductoTipoCode);
        }

        hc.Date = data.Date.Date;
        hc.IsModifiable = data.IsModifiable;

        await HojaCorteReader.SaveAsync(_uow, ct);

        return await HojaCorteReader.GetAsync(_uow, year, code, ct);
    }
}
