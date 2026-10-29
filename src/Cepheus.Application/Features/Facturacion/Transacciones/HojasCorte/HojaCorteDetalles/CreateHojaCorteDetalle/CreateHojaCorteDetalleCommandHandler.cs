using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.HojaCorteDetalles.CreateHojaCorteDetalle;

public sealed class CreateHojaCorteDetalleCommandHandler
    : IRequestHandler<CreateHojaCorteDetalleCommand, HojaCorteResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateHojaCorteDetalleCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<HojaCorteResponse> Handle(CreateHojaCorteDetalleCommand request, CancellationToken ct)
    {
        var year = HojaCorteRules.Normalize(request.Year);
        var code = HojaCorteRules.NormalizeCode(request.Code);
        var d = request.Data;
        var tipo = d.ProductoTipoCode.Trim();
        var producto = HojaCorteLineas.NormalizeProductoCode(d.ProductoCode);

        var hc = await HojaCorteReader.LoadForUpdateAsync(_uow, year, code, ct);
        HojaCorteRules.EnsureEditable(hc);

        HojaCorteLineas.EnsureShape(tipo, d);
        HojaCorteLineas.EnsureConsistent(hc, tipo);

        var resuelta = await HojaCorteLineas.ResolverAsync(_uow, tipo, producto, d.UnitCode, ct);

        hc.Detalles.Add(HojaCorteLineas.Build(
            hc, tipo, producto, HojaCorteLineas.NextItem(hc, tipo), resuelta.UnitCode, d));

        await HojaCorteReader.SaveAsync(_uow, ct);

        return await HojaCorteReader.GetAsync(_uow, year, code, ct);
    }
}
