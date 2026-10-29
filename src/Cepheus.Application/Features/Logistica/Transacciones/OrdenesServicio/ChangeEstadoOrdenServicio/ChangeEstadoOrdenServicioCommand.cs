using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.ChangeEstadoOrdenServicio;

/// <summary>
/// Cambia el estado: Aprobado (valida al aprobador contra la matriz), Cerrado o Anulado.
/// Para pasar a Procesado se usa ProcesarOrdenServicioCommand (mueve costos de stock y materiales de OT).
/// NuevoEstado acepta el nombre del enum (Aprobado, Cerrado, Anulado) o el código legacy (09, 11, 04).
/// </summary>
public sealed record ChangeEstadoOrdenServicioCommand(string PlantaCode, string Code, string NuevoEstado)
    : IRequest<OrdenServicioResponse>;
