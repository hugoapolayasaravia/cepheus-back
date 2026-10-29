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

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.CreateComprobanteVentaLibre;

/// <summary>
/// Comprobante libre (PB tipo 'O', consumos varios): sin guías ni OT. Su número no puede ser uno aprobado por Créditos.
/// </summary>
public sealed class CreateComprobanteVentaLibreCommand : IRequest<ComprobanteVentaResponse>
{
    public string TypeCode { get; set; } = default!;
    public string? Number { get; set; }
    public string PlantCode { get; set; } = default!;
    public string? ExecutingPlantCode { get; set; }
    public string ClienteCode { get; set; } = default!;
    public string ObraCode { get; set; } = default!;
    public string? VendedorCode { get; set; }

    /// <summary>Moneda: PEN o USD (por defecto PEN).</summary>
    public string? CurrencyCode { get; set; }

    public string? Address { get; set; }
    public string? UbigeoCode { get; set; }
    public DateTime? IssueDate { get; set; }
    public string Glosa { get; set; } = default!;

    public List<ComprobanteVentaLineaLibreRequest> Detalles { get; set; } = new();
}
