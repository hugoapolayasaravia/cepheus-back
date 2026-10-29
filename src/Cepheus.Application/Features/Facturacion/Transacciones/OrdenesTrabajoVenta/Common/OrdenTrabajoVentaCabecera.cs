using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

/// <summary>Aplica los datos modificables de la cabecera (comunes a Create y Update) validando contra los maestros.</summary>
public static class OrdenTrabajoVentaCabecera
{
    public static async Task<Obra> GetObraAsync(IUnitOfWork uow, string clienteCode, string obraCode, CancellationToken ct)
        => await uow.Facturacion.Maestros.Obras
               .Query().AsNoTracking()
               .FirstOrDefaultAsync(o => o.ClienteCode == clienteCode && o.Code == obraCode, ct)
           ?? throw new KeyNotFoundException($"La obra {clienteCode}/{obraCode} no existe.");

    public static async Task ApplyAsync(
        IUnitOfWork uow,
        OrdenTrabajoVenta ot,
        OrdenTrabajoVentaUpdateFields f,
        Obra obra,
        string? cotizacionCurrency,
        CancellationToken ct)
    {
        if (!OrdenTrabajoVentaRules.IsValidTime(f.DispatchTime))
        {
            throw new InvalidOperationException("La hora de despacho no es válida (HH:mm, hora 01-23 y minuto 00-59).");
        }

        if (string.IsNullOrWhiteSpace(obra.ResponsibleName))
        {
            throw new InvalidOperationException("La obra no tiene responsable registrado: complételo en el maestro de obras.");
        }

        if (string.IsNullOrWhiteSpace(obra.DeliveryAddress) && string.IsNullOrWhiteSpace(obra.Address))
        {
            throw new InvalidOperationException("La obra no tiene dirección de entrega registrada.");
        }

        var cliente = await uow.Facturacion.Maestros.Clientes
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == ot.ClienteCode, ct)
            ?? throw new KeyNotFoundException($"El cliente {ot.ClienteCode} no existe.");

        var purchaseOrder = OrdenTrabajoVentaRules.NormalizeText(f.PurchaseOrderNumber);

        if (purchaseOrder is null
            && cliente.RequiresPurchaseOrderApproval == true
            && ot.Type is Cepheus.Domain.Facturacion.Enum.TipoOrdenTrabajoVenta.Normal
                or Cepheus.Domain.Facturacion.Enum.TipoOrdenTrabajoVenta.Anticipo)
        {
            throw new InvalidOperationException("El cliente exige N° de orden de compra para sus OT.");
        }

        var vendedorCode = OrdenTrabajoVentaRules.NormalizeFixedOrNull(f.VendedorCode, 4) ?? obra.VendedorCode;

        var vendedor = await uow.Facturacion.Maestros.Vendedores
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(v => v.Code == vendedorCode, ct)
            ?? throw new KeyNotFoundException($"El vendedor {vendedorCode} no existe.");

        if (!vendedor.IsActive)
        {
            throw new InvalidOperationException($"El vendedor {vendedorCode} está inactivo.");
        }

        var motivoCode = OrdenTrabajoVentaRules.Normalize(f.MotivoCode);

        var motivo = await uow.Comunes.MotivosDevolucion
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(m => m.Code == motivoCode, ct)
            ?? throw new KeyNotFoundException($"El motivo {motivoCode} no existe.");

        if (!motivo.IsActive)
        {
            throw new InvalidOperationException($"El motivo {motivoCode} está inactivo.");
        }

        var currency = OrdenTrabajoVentaRules.NormalizeOrNull(f.CurrencyCode) ?? cotizacionCurrency;

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 1)
        {
            throw new InvalidOperationException("La moneda es obligatoria (un carácter, como en el legacy).");
        }

        ot.DispatchDate = f.DispatchDate.Date;
        ot.DispatchTime = f.DispatchTime.Trim();
        ot.VendedorCode = vendedorCode;
        ot.MotivoCode = motivoCode;
        ot.CurrencyCode = currency;
        ot.PurchaseOrderNumber = purchaseOrder;
        ot.Observations = OrdenTrabajoVentaRules.NormalizeText(f.Observations);
        ot.CutSheetObservations = OrdenTrabajoVentaRules.NormalizeText(f.CutSheetObservations);
    }
}
