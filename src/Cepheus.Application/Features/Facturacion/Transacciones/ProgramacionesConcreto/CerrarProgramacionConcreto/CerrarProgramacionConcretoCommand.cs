using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.CerrarProgramacionConcreto;

/// <summary>
/// Cierra la programación (PB: ges_programacion_cierre): lo programado y no entregado vuelve a estar disponible en la OT
/// (cada línea queda con lo entregado) y la programación pasa a Cerrada.
/// </summary>
public sealed record CerrarProgramacionConcretoCommand(string Code) : IRequest<ProgramacionConcretoResponse>;
