using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.CreateProgramacionConcreto;

/// <summary>Crea una programación de Concreto (parcial) de una OT aprobada y con pase a producción.</summary>
public sealed class CreateProgramacionConcretoCommand : ProgramacionConcretoFields, IRequest<ProgramacionConcretoResponse>
{
    public string OrdenTrabajoCode { get; set; } = default!;

    /// <summary>Líneas de la OT que se programan (obligatorio al menos una; un solo concreto tipo 01).</summary>
    public List<ProgramacionConcretoDetalleRequest> Detalles { get; set; } = new();
}
