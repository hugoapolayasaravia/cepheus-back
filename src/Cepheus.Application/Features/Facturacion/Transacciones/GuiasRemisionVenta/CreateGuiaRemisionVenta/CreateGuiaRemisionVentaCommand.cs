using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.CreateGuiaRemisionVenta;

/// <summary>
/// Emite una guía de remisión. Con OT (venta/reposición) se genera desde una programación Pendiente de la OT; la
/// devolución exige OT sin programación; el traslado y la guía "en espera" no llevan OT.
/// </summary>
public sealed class CreateGuiaRemisionVentaCommand : GuiaRemisionVentaFields, IRequest<GuiaRemisionVentaResponse>
{
    /// <summary>1 Venta | 2 Traslado | 10 Devolución | 13 Reposición.</summary>
    public int Motive { get; set; } = (int)MotivoGuiaRemisionVenta.Venta;

    /// <summary>Número SSS-NNNNNN; si se omite se toma el siguiente de la planta.</summary>
    public string? Number { get; set; }

    public string? OrdenTrabajoCode { get; set; }

    /// <summary>"concreto" u "otros". Obligatorio con OT en venta/reposición.</summary>
    public string? ProgramacionType { get; set; }

    public string? ProgramacionCode { get; set; }

    /// <summary>Planta. Con programación se toma la de la programación; es obligatoria sin OT y en devolución.</summary>
    public string? PlantCode { get; set; }

    /// <summary>Solo sin OT.</summary>
    public string? ClienteCode { get; set; }

    /// <summary>Solo sin OT.</summary>
    public string? ObraCode { get; set; }

    /// <summary>Solo sin OT (por defecto el vendedor de la obra).</summary>
    public string? VendedorCode { get; set; }

    /// <summary>Reposición (13): número de la guía que se repone.</summary>
    public string? ReplacesNumber { get; set; }

    /// <summary>Devolución (10): motivo de devolución.</summary>
    public string? DevolutionMotiveCode { get; set; }

    /// <summary>Devolución (10): acción; "01" devuelve la cantidad al saldo de la OT.</summary>
    public string? DevolutionActionCode { get; set; }

    public List<GuiaRemisionVentaDetalleRequest> Detalles { get; set; } = new();
}
