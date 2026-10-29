using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.CreateComprobanteVenta;

/// <summary>
/// Emite una Factura (01) o Boleta (03) con el detalle de guías Pendientes de venta del mismo cliente, obra, OT y moneda.
/// Si las guías ya traen número de documento (OT al contado) el comprobante lleva ese número.
/// </summary>
public sealed class CreateComprobanteVentaCommand : IRequest<ComprobanteVentaResponse>
{
    /// <summary>"01" Factura | "03" Boleta.</summary>
    public string TypeCode { get; set; } = default!;

    /// <summary>SSS-NNNNNN; si se omite, el de las guías o el siguiente de la planta.</summary>
    public string? Number { get; set; }

    /// <summary>Planta que emite (por defecto la de la primera guía).</summary>
    public string? PlantCode { get; set; }

    /// <summary>Planta ejecutora (por defecto la que emite).</summary>
    public string? ExecutingPlantCode { get; set; }

    /// <summary>Fecha de emisión (por defecto hoy; no puede ser futura ni de un mes cerrado).</summary>
    public DateTime? IssueDate { get; set; }

    public string? Glosa { get; set; }

    /// <summary>Números de las guías que se facturan (al menos una).</summary>
    public List<string> Guias { get; set; } = new();
}
