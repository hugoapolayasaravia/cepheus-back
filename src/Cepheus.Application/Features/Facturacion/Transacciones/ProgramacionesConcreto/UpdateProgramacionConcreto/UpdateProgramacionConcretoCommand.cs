using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.UpdateProgramacionConcreto;

public sealed class UpdateProgramacionConcretoRequest : ProgramacionConcretoFields
{
    /// <summary>
    /// Conjunto final de líneas: las que ya estaban se actualizan, las nuevas se agregan y las que no vengan se quitan
    /// (PB: eliminar una línea = cantidad 0).
    /// </summary>
    public List<ProgramacionConcretoDetalleRequest> Detalles { get; set; } = new();
}

/// <summary>Modifica una programación Pendiente y sin guías (PB: gs_accion = 'M').</summary>
public sealed record UpdateProgramacionConcretoCommand(string Code, UpdateProgramacionConcretoRequest Data)
    : IRequest<ProgramacionConcretoResponse>;
