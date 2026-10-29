using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.UpdateProgramacionOtros;

public sealed class UpdateProgramacionOtrosRequest : ProgramacionOtrosFields
{
    /// <summary>Conjunto final de líneas: se actualizan, agregan o quitan (PB: quitar = cantidad 0).</summary>
    public List<ProgramacionOtrosDetalleRequest> Detalles { get; set; } = new();
}

/// <summary>Modifica una programación Pendiente y sin guías.</summary>
public sealed record UpdateProgramacionOtrosCommand(string Code, UpdateProgramacionOtrosRequest Data)
    : IRequest<ProgramacionOtrosResponse>;
