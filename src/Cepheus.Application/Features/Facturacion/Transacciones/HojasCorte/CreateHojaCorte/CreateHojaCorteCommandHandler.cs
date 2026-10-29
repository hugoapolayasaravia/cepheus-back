using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.CreateHojaCorte;

public sealed class CreateHojaCorteCommandHandler : IRequestHandler<CreateHojaCorteCommand, HojaCorteResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateHojaCorteCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<HojaCorteResponse> Handle(CreateHojaCorteCommand request, CancellationToken ct)
    {
        var date = HojaCorteRules.GetProcessDate();
        var year = HojaCorteRules.YearOf(date);
        var code = await HojaCorteRules.NextCodeAsync(_uow, year, ct);

        var hc = new HojaCorte
        {
            Year = year,
            Code = code,
            Date = date,
            Status = EstadoHojaCorte.Pendiente,
            IsModifiable = true,
            IsScheduled = false
        };

        await HojaCorteCabecera.ApplyAsync(_uow, hc, request, ct);
        await HojaCorteCabecera.ApplyParentAsync(_uow, hc, request.ParentYear, request.ParentCode, ct);

        foreach (var d in request.Detalles)
        {
            var tipo = d.ProductoTipoCode.Trim();
            var producto = HojaCorteLineas.NormalizeProductoCode(d.ProductoCode);

            HojaCorteLineas.EnsureShape(tipo, d);
            HojaCorteLineas.EnsureConsistent(hc, tipo);

            var resuelta = await HojaCorteLineas.ResolverAsync(_uow, tipo, producto, d.UnitCode, ct);

            hc.Detalles.Add(HojaCorteLineas.Build(
                hc, tipo, producto, HojaCorteLineas.NextItem(hc, tipo), resuelta.UnitCode, d));
        }

        await _uow.Facturacion.Transacciones.HojasCorte.AddAsync(hc, ct);
        await HojaCorteReader.SaveAsync(_uow, ct);

        return await HojaCorteReader.GetAsync(_uow, year, code, ct);
    }
}
