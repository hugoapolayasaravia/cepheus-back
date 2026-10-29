using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.AnularProgramacionConcreto;

/// <summary>
/// Anula la programación (PB: estado A / codigo_est 04): libera la cantidad programada de la OT; si no queda ninguna
/// programación la OT vuelve a etapa C. Una programación con guías no se anula.
/// </summary>
public sealed record AnularProgramacionConcretoCommand(string Code) : IRequest<ProgramacionConcretoResponse>;
