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

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.UpdateGuiaRemisionVenta;

public sealed class UpdateGuiaRemisionVentaRequest : GuiaRemisionVentaFields
{
    /// <summary>Conjunto final de líneas (con OT por Item; sin OT por producto).</summary>
    public List<GuiaRemisionVentaDetalleRequest> Detalles { get; set; } = new();
}

/// <summary>Modifica una guía Pendiente y sin imprimir (PB: ue_set_btn_mod_dw). No cambia OT, programación, motivo ni cliente.</summary>
public sealed record UpdateGuiaRemisionVentaCommand(string Number, UpdateGuiaRemisionVentaRequest Data)
    : IRequest<GuiaRemisionVentaResponse>;
