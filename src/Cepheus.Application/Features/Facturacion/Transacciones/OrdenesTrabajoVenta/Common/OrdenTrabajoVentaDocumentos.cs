using System.Globalization;
using System.Text.RegularExpressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

/// <summary>
/// Numeración de documentos de pago por planta (PB: fc_jaladoc, MPlantas.numero_fve / numero_bve).
/// El número es "SSS-NNNNNN" (serie de 3 + guion + correlativo de 6 = 10 caracteres, como Numero_fve char(10)).
/// Planta.NumeroFve / NumeroBve guardan el ÚLTIMO número usado; el siguiente es el último + 1.
/// </summary>
public static class OrdenTrabajoVentaDocumentos
{
    private static readonly Regex NumberRegex = new(@"^(\d{3})-(\d{6})$", RegexOptions.Compiled);

    public static bool IsValid(string? number) => number is not null && NumberRegex.IsMatch(number);

    public static string GetLast(Planta planta, TipoDocumentoAprobacion type)
        => type == TipoDocumentoAprobacion.Factura ? planta.NumeroFve : planta.NumeroBve;

    public static void SetLast(Planta planta, TipoDocumentoAprobacion type, string number)
    {
        if (type == TipoDocumentoAprobacion.Factura)
        {
            planta.NumeroFve = number;
        }
        else
        {
            planta.NumeroBve = number;
        }
    }

    /// <summary>Número siguiente al indicado (misma serie).</summary>
    public static string Next(string last)
    {
        var m = NumberRegex.Match(last ?? string.Empty);

        if (!m.Success)
        {
            throw new InvalidOperationException($"La numeración de la planta '{last}' no tiene el formato SSS-NNNNNN.");
        }

        var n = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture) + 1;

        if (n > 999999)
        {
            throw new InvalidOperationException($"Se agotó la numeración de la serie {m.Groups[1].Value}.");
        }

        return $"{m.Groups[1].Value}-{n.ToString("000000", CultureInfo.InvariantCulture)}";
    }

    /// <summary>Número anterior al indicado (misma serie), o null si ya es el primero.</summary>
    public static string? Previous(string number)
    {
        var m = NumberRegex.Match(number ?? string.Empty);

        if (!m.Success)
        {
            return null;
        }

        var n = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture) - 1;

        return n < 0 ? null : $"{m.Groups[1].Value}-{n.ToString("000000", CultureInfo.InvariantCulture)}";
    }

    /// <summary>
    /// true si el número ya está tomado por la aprobación de una OT de OTRO cliente (PB: ue_validardocumento, que
    /// compara Aprobaciones.documento / numero_doc2 contra los pedidos de clientes distintos). Un mismo cliente puede
    /// repetir el documento en varias OT.
    /// </summary>
    public static Task<bool> ExistsForOtherClientAsync(
        IUnitOfWork uow,
        TipoDocumentoAprobacion type,
        string number,
        string clienteCode,
        CancellationToken ct)
        => uow.Facturacion.Transacciones.OrdenesTrabajoVentaAprobaciones
            .Query()
            .AsNoTracking()
            .AnyAsync(a => a.DocumentType == type
                           && (a.Document1Number == number || a.Document2Number == number)
                           && a.OrdenTrabajo.ClienteCode != clienteCode, ct);

    /// <summary>true si el número ya está en la aprobación de OTRA OT (cualquier cliente). Se usa al asignar uno automático.</summary>
    public static Task<bool> ExistsInOtherOrderAsync(
        IUnitOfWork uow,
        TipoDocumentoAprobacion type,
        string number,
        string otCode,
        CancellationToken ct)
        => uow.Facturacion.Transacciones.OrdenesTrabajoVentaAprobaciones
            .Query()
            .AsNoTracking()
            .AnyAsync(a => a.DocumentType == type
                           && (a.Document1Number == number || a.Document2Number == number)
                           && a.OrdenTrabajoCode != otCode, ct);

    /// <summary>
    /// Toma el número de documento de la planta: el indicado (validado) o el siguiente al último. Avanza el último
    /// número de la planta si corresponde. La planta debe estar tracked para que se guarde junto con la aprobación.
    /// </summary>
    public static async Task<string> TakeAsync(
        IUnitOfWork uow,
        Planta planta,
        TipoDocumentoAprobacion type,
        string? requested,
        string otCode,
        string clienteCode,
        CancellationToken ct)
    {
        var last = GetLast(planta, type);
        string number;

        if (string.IsNullOrWhiteSpace(requested))
        {
            number = Next(last);

            if (await ExistsInOtherOrderAsync(uow, type, number, otCode, ct))
            {
                throw new InvalidOperationException(
                    $"El número siguiente de la planta ({number}) ya está asignado a otra Orden de Trabajo; indique el número a mano.");
            }
        }
        else
        {
            number = requested.Trim();

            if (!IsValid(number))
            {
                throw new InvalidOperationException($"El número de documento '{number}' debe tener el formato SSS-NNNNNN.");
            }

            if (await ExistsForOtherClientAsync(uow, type, number, clienteCode, ct))
            {
                throw new InvalidOperationException($"El número de documento {number} ya está registrado en la aprobación de otro cliente.");
            }
        }

        if (string.CompareOrdinal(number, last) > 0)
        {
            SetLast(planta, type, number);
        }

        return number;
    }

    /// <summary>
    /// Al anular una aprobación: SOLO la Factura devuelve su número (PB: "update mplantas set numero_fve"), y solo si
    /// es el último de la planta. Las boletas no retroceden el correlativo.
    /// </summary>
    public static void Release(Planta? planta, TipoDocumentoAprobacion type, string? number)
    {
        if (type != TipoDocumentoAprobacion.Factura || planta is null || string.IsNullOrWhiteSpace(number))
        {
            return;
        }

        if (GetLast(planta, type) != number)
        {
            return;
        }

        var previous = Previous(number);

        if (previous is not null)
        {
            SetLast(planta, type, previous);
        }
    }

    /// <summary>
    /// true si ya se emitió el comprobante de un documento de Contado (PB: rFacVentas/rBolVentas con estado 01, 02 o 03).
    /// Consulta los comprobantes de pago (ComprobanteVenta) del tipo aprobado cuyo número sea el del documento 1 o 2 de
    /// la aprobación y que no estén anulados.
    /// </summary>
    public static async Task<bool> IsIssuedAsync(IUnitOfWork uow, OrdenTrabajoVentaAprobacion approval, CancellationToken ct)
    {
        var typeCode = approval.DocumentType == TipoDocumentoAprobacion.Factura ? "01" : "03";
        var d1 = approval.Document1Number;
        var d2 = approval.Document2Number;

        if (string.IsNullOrWhiteSpace(d1) && string.IsNullOrWhiteSpace(d2))
        {
            return false;
        }

        return await uow.Facturacion.Transacciones.ComprobantesVenta
            .Query().AsNoTracking()
            .AnyAsync(c => c.TypeCode == typeCode
                           && c.Status != EstadoComprobanteVenta.Anulado
                           && (c.Number == d1 || c.Number == d2), ct);
    }
}
