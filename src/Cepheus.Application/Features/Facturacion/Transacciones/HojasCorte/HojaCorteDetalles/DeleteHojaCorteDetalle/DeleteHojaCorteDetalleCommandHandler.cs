using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.HojaCorteDetalles.DeleteHojaCorteDetalle;

public sealed class DeleteHojaCorteDetalleCommandHandler
    : IRequestHandler<DeleteHojaCorteDetalleCommand, HojaCorteResponse>
{
    private readonly IUnitOfWork _uow;

    public DeleteHojaCorteDetalleCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<HojaCorteResponse> Handle(DeleteHojaCorteDetalleCommand request, CancellationToken ct)
    {
        var year = HojaCorteRules.Normalize(request.Year);
        var code = HojaCorteRules.NormalizeCode(request.Code);
        var tipo = request.ProductoTipoCode.Trim();
        var producto = HojaCorteLineas.NormalizeProductoCode(request.ProductoCode);

        var hc = await HojaCorteReader.LoadForUpdateAsync(_uow, year, code, ct);
        HojaCorteRules.EnsureEditable(hc);

        var linea = hc.Detalles.FirstOrDefault(d =>
                d.ProductoTipoCode == tipo && d.ProductoCode == producto && d.ItemNumber == request.ItemNumber)
            ?? throw new KeyNotFoundException(
                $"La línea {tipo}/{producto} ítem {request.ItemNumber} no existe en la Hoja de Corte {year}/{code}.");

        hc.Detalles.Remove(linea);
        _uow.Facturacion.Transacciones.HojasCorteDetalle.Remove(linea);

        await HojaCorteReader.SaveAsync(_uow, ct);

        return await HojaCorteReader.GetAsync(_uow, year, code, ct);
    }
}
