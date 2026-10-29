using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.CopiarHojaCorte;

public sealed class CopiarHojaCorteCommandHandler : IRequestHandler<CopiarHojaCorteCommand, HojaCorteResponse>
{
    private readonly IUnitOfWork _uow;

    public CopiarHojaCorteCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<HojaCorteResponse> Handle(CopiarHojaCorteCommand request, CancellationToken ct)
    {
        var originYear = HojaCorteRules.Normalize(request.Year);
        var originCode = HojaCorteRules.NormalizeCode(request.Code);

        var origen = await _uow.Facturacion.Transacciones.HojasCorte
            .Query()
            .AsNoTracking()
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync(x => x.Year == originYear && x.Code == originCode, ct)
            ?? throw new KeyNotFoundException($"La Hoja de Corte {originYear}/{originCode} no existe.");

        var date = HojaCorteRules.GetProcessDate();
        var year = HojaCorteRules.YearOf(date);
        var code = await HojaCorteRules.NextCodeAsync(_uow, year, ct);

        var copia = new HojaCorte
        {
            Year = year,
            Code = code,
            Date = date,
            Status = EstadoHojaCorte.Pendiente,
            IsModifiable = true,
            IsScheduled = false,

            TipoHojaCorteCode = origen.TipoHojaCorteCode,
            ClienteCode = origen.ClienteCode,
            ClientName = origen.ClientName,
            ObraCode = origen.ObraCode,
            WorkName = origen.WorkName,
            WorkAddress = origen.WorkAddress,
            WorkAddressUbigeoCode = origen.WorkAddressUbigeoCode,
            TecnicoCode = origen.TecnicoCode,
            VendedorCode = origen.VendedorCode,
            Sector = origen.Sector,
            Floor = origen.Floor,
            ClientViguetaDispatchDate = origen.ClientViguetaDispatchDate,
            ClientBovedillaDispatchDate = origen.ClientBovedillaDispatchDate,
            StorageDate = origen.StorageDate,
            Observations = origen.Observations,
            Turn = origen.Turn,
            Motivo = origen.Motivo,
            ProductType = origen.ProductType,

            // Origen de la copia (decisión del usuario).
            ParentYear = origen.Year,
            ParentCode = origen.Code
        };

        // No se copian: despachos reales (Viguetas/Bovedillas), OT, estado ni programación.
        foreach (var d in origen.Detalles.OrderBy(x => x.ProductoTipoCode).ThenBy(x => x.ItemNumber))
        {
            copia.Detalles.Add(new HojaCorteDetalle
            {
                Year = year,
                HojaCorteCode = code,
                ProductoTipoCode = d.ProductoTipoCode,
                ProductoCode = d.ProductoCode,
                ItemNumber = d.ItemNumber,
                UnitCode = d.UnitCode,
                Quantity = d.Quantity,
                Panel = d.Panel,
                Length = d.Length,
                CablesSide1 = d.CablesSide1,
                CablesSide2 = d.CablesSide2,
                Lot1 = d.Lot1,
                Lot2 = d.Lot2,
                Lot3 = d.Lot3
            });
        }

        await _uow.Facturacion.Transacciones.HojasCorte.AddAsync(copia, ct);
        await HojaCorteReader.SaveAsync(_uow, ct);

        return await HojaCorteReader.GetAsync(_uow, year, code, ct);
    }
}
