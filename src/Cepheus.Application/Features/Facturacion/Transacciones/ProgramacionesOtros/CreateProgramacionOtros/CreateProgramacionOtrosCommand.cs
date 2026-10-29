using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.CreateProgramacionOtros;

/// <summary>Crea una programación de Otros (parcial) de una OT aprobada y con pase a producción.</summary>
public sealed class CreateProgramacionOtrosCommand : ProgramacionOtrosFields, IRequest<ProgramacionOtrosResponse>
{
    public string OrdenTrabajoCode { get; set; } = default!;

    /// <summary>Líneas de la OT (no Concreto) que se programan; al menos una.</summary>
    public List<ProgramacionOtrosDetalleRequest> Detalles { get; set; } = new();
}
