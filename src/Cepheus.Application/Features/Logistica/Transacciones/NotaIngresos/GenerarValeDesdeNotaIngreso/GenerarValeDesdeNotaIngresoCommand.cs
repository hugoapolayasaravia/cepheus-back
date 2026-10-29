using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GenerarValeDesdeNotaIngreso;

/// <summary>
/// Botón "Vales" de la Nota de Ingreso: genera un Vale de Salida de tipo Transferencia (el legacy fija el tipo 'TRA')
/// con las líneas de la nota, para enviarlas a otra planta. El vale nace Pendiente, en la misma planta de la nota.
/// </summary>
public sealed record GenerarValeDesdeNotaIngresoCommand(
    string PlantaCode,
    string NotaIngresoCode,
    DateTime FechaEntrega,
    string SubCentroCostoCode,
    string TrabajadorCode,
    string UnidadNegocioCode,
    string PlantaAfectadaCode) : IRequest<ValeResponse>;
