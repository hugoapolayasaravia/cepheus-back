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

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.AnularGuiaRemisionVenta;

public sealed class AnularGuiaRemisionVentaRequest
{
    /// <summary>Motivo de la anulación (obligatorio).</summary>
    public string Reason { get; set; } = default!;
}

/// <summary>Anula una guía Pendiente (PB: fa_anula_gve). La aprobación del PB es el permiso GUIASREMISIONVENTA.ANNUL.</summary>
public sealed record AnularGuiaRemisionVentaCommand(string Number, AnularGuiaRemisionVentaRequest Data)
    : IRequest<GuiaRemisionVentaResponse>;
