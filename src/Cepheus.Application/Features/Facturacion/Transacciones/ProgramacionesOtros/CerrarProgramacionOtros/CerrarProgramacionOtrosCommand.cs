using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.CerrarProgramacionOtros;

/// <summary>
/// Cierra la programación (PB: ges_programacion_cierre_otros): lo no entregado vuelve a estar disponible en la OT
/// (cada línea queda con lo entregado) y la programación pasa a Cerrada.
/// </summary>
public sealed record CerrarProgramacionOtrosCommand(string Code) : IRequest<ProgramacionOtrosResponse>;
