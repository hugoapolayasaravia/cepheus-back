using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;

/// <summary>
/// Aplica a la entidad los datos de cabecera (alta y modificación) validando contra los catálogos y maestros.
/// </summary>
public static class HojaCorteCabecera
{
    public static string NormalizeTipoCode(string value)
    {
        var v = value.Trim();
        return v.Length == 1 && char.IsDigit(v[0]) ? v.PadLeft(2, '0') : v.ToUpperInvariant();
    }

    public static async Task ApplyAsync(
        IUnitOfWork uow,
        HojaCorte hc,
        HojaCorteHeaderRequest r,
        CancellationToken ct)
    {
        // ---- tipo de hoja de corte (catálogo)
        var tipoCode = NormalizeTipoCode(r.TipoHojaCorteCode);

        var tipo = await uow.Facturacion.Catalogos.TiposHojaCorte
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == tipoCode, ct)
            ?? throw new KeyNotFoundException($"El tipo de Hoja de Corte {tipoCode} no existe.");

        if (!tipo.IsActive)
        {
            throw new InvalidOperationException($"El tipo de Hoja de Corte {tipoCode} está inactivo.");
        }

        // ---- cliente / obra (opcionales, como en el PB)
        var clienteCode = HojaCorteRules.NormalizeFixedOrNull(r.ClienteCode, 5);
        var obraCode = HojaCorteRules.NormalizeFixedOrNull(r.ObraCode, 3);

        Cepheus.Domain.Facturacion.Maestros.Cliente? cliente = null;
        Cepheus.Domain.Facturacion.Maestros.Obra? obra = null;

        if (clienteCode is not null)
        {
            cliente = await uow.Facturacion.Maestros.Clientes
                .Query().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == clienteCode, ct)
                ?? throw new KeyNotFoundException($"El cliente {clienteCode} no existe.");

            if (cliente.Estado != EstadoCliente.Activo)
            {
                throw new InvalidOperationException($"El cliente {clienteCode} no está activo.");
            }
        }

        if (obraCode is not null)
        {
            if (clienteCode is null)
            {
                throw new InvalidOperationException("Para indicar una obra debe indicar primero el cliente.");
            }

            obra = await uow.Facturacion.Maestros.Obras
                .Query().AsNoTracking()
                .FirstOrDefaultAsync(x => x.ClienteCode == clienteCode && x.Code == obraCode, ct)
                ?? throw new KeyNotFoundException(
                    $"La obra {obraCode} del cliente {clienteCode} no existe.");

            if (obra.Estado != EstadoObra.Activo)
            {
                throw new InvalidOperationException($"La obra {obraCode} no está activa.");
            }
        }

        // ---- snapshot: lo enviado manda; si no se envía se toma del maestro
        var clientName = HojaCorteRules.NormalizeOrNull(r.ClientName) ?? cliente?.Name?.Trim().ToUpperInvariant();
        var workName = HojaCorteRules.NormalizeOrNull(r.WorkName) ?? obra?.Description.Trim().ToUpperInvariant() ?? string.Empty;
        var workAddress = HojaCorteRules.NormalizeOrNull(r.WorkAddress) ?? obra?.Address.Trim().ToUpperInvariant() ?? string.Empty;
        var ubigeoCode = HojaCorteRules.NormalizeOrNull(r.WorkAddressUbigeoCode) ?? obra?.UbigeoCode;

        if (string.IsNullOrWhiteSpace(clientName))
        {
            throw new InvalidOperationException("Debe indicar el nombre del cliente.");
        }

        if (string.IsNullOrWhiteSpace(ubigeoCode))
        {
            throw new InvalidOperationException("Debe indicar el ubigeo de la dirección de la obra.");
        }

        var ubigeoOk = await uow.Comunes.Ubigeos.Query().AsNoTracking()
            .AnyAsync(x => x.Code == ubigeoCode, ct);

        if (!ubigeoOk)
        {
            throw new KeyNotFoundException($"El ubigeo {ubigeoCode} no existe.");
        }

        // ---- técnico y vendedor
        var tecnicoCode = HojaCorteRules.NormalizeFixed(r.TecnicoCode, 5);

        var tecnico = await uow.Facturacion.Maestros.Tecnicos
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.TrabajadorCode == tecnicoCode, ct)
            ?? throw new KeyNotFoundException($"El técnico {tecnicoCode} no existe.");

        if (!tecnico.IsActive)
        {
            throw new InvalidOperationException($"El técnico {tecnicoCode} está inactivo.");
        }

        var vendedorCode = HojaCorteRules.NormalizeFixedOrNull(r.VendedorCode, 4) ?? obra?.VendedorCode;

        if (string.IsNullOrWhiteSpace(vendedorCode))
        {
            throw new InvalidOperationException("Debe indicar el vendedor (o seleccionar una obra con vendedor).");
        }

        var vendedor = await uow.Facturacion.Maestros.Vendedores
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == vendedorCode, ct)
            ?? throw new KeyNotFoundException($"El vendedor {vendedorCode} no existe.");

        if (!vendedor.IsActive)
        {
            throw new InvalidOperationException($"El vendedor {vendedorCode} está inactivo.");
        }

        // ---- motivo solo en Reemplazo
        if (TipoHojaCorteCodes.RequiresMotivo(tipoCode))
        {
            if (r.Motivo is null)
            {
                throw new InvalidOperationException("El motivo es obligatorio en una Hoja de Corte de tipo Reemplazo.");
            }

            hc.Motivo = r.Motivo;
        }
        else
        {
            hc.Motivo = null;
        }

        hc.TipoHojaCorteCode = tipoCode;
        hc.ClienteCode = clienteCode;
        hc.ClientName = clientName;
        hc.ObraCode = obraCode;
        hc.WorkName = workName;
        hc.WorkAddress = workAddress;
        hc.WorkAddressUbigeoCode = ubigeoCode;
        hc.TecnicoCode = tecnicoCode;
        hc.VendedorCode = vendedorCode;
        hc.Sector = HojaCorteRules.NormalizeOrNull(r.Sector) ?? string.Empty;
        hc.Floor = HojaCorteRules.Normalize(r.Floor);
        hc.ClientViguetaDispatchDate = r.ClientViguetaDispatchDate;
        hc.ClientBovedillaDispatchDate = r.ClientBovedillaDispatchDate;
        hc.StorageDate = r.StorageDate?.Date;
        hc.Observations = string.IsNullOrWhiteSpace(r.Observations) ? null : r.Observations.Trim();
        hc.Turn = r.Turn;
        hc.ProductType = r.ProductType;
    }

    /// <summary>
    /// Hoja de Corte padre: obligatoria en Adicional (03) y Reemplazo (05); no se admite en los demás tipos.
    /// </summary>
    public static async Task ApplyParentAsync(
        IUnitOfWork uow,
        HojaCorte hc,
        string? parentYear,
        string? parentCode,
        CancellationToken ct)
    {
        var requires = TipoHojaCorteCodes.RequiresParent(hc.TipoHojaCorteCode);
        var year = HojaCorteRules.NormalizeOrNull(parentYear);
        var code = HojaCorteRules.NormalizeCodeOrNull(parentCode);

        if (!requires)
        {
            if (year is not null || code is not null)
            {
                throw new InvalidOperationException(
                    "Solo las Hojas de Corte de tipo Adicional o Reemplazo admiten una Hoja de Corte padre.");
            }

            hc.ParentYear = null;
            hc.ParentCode = null;
            return;
        }

        if (year is null || code is null)
        {
            throw new InvalidOperationException(
                "Debe indicar la Hoja de Corte padre (ParentYear y ParentCode) para este tipo de Hoja de Corte.");
        }

        var parent = await uow.Facturacion.Transacciones.HojasCorte
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Year == year && x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Hoja de Corte padre {year}/{code} no existe.");

        if (parent.Status == EstadoHojaCorte.Anulado)
        {
            throw new InvalidOperationException($"La Hoja de Corte padre {year}/{code} está Anulada.");
        }

        hc.ParentYear = year;
        hc.ParentCode = code;
    }
}
