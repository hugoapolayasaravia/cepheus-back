using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Transferencias.GenerarTransferencia;

/// <summary>
/// Recibe en el almacén destino un vale de transferencia: genera la Nota de Ingreso (procesada), suma el stock del
/// destino, resta el del origen y deja el vale procesado, todo en una sola transacción.
/// FechaEmision es opcional: por defecto la fecha de recepción (como la pantalla del legacy).
/// </summary>
public sealed record GenerarTransferenciaCommand(
    string PlantaDestinoCode,
    string PlantaOrigenCode,
    string ValeCode,
    DateTime FechaRecepcion,
    DateTime? FechaEmision) : IRequest<NotaIngresoResponse>;
