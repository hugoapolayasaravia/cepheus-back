using MediatR;

namespace Cepheus.Application.Features.Logistica.Reportes.InventarioPermanente.GetReporteInventarioPermanenteValorizado;

/// <summary>
/// Inventario Permanente Valorizado (legacy Logi_sp_Rpt_Inventario_Permanente_Valorizado): Kardex valorizado por
/// costo promedio móvil de todos los artículos de una planta entre dos fechas. Reutiliza el valorizador del Kardex
/// y del cierre mensual, por lo que sus números coinciden. Sin tablas temporales: varios usuarios pueden consultarlo a la vez.
/// </summary>
/// <param name="FamiliaCode">Familia (o inicio del rango de familias si se envía también FamiliaHastaCode).</param>
/// <param name="FamiliaHastaCode">Fin del rango de familias (BETWEEN del legacy).</param>
/// <param name="SubFamiliaCode">Subfamilia (si se envía también SubFamilia2Code, se incluyen ambas).</param>
/// <param name="Resumen">false = detalle por movimiento (Opcion 0 del legacy); true = un renglón por artículo (resumen).</param>
public sealed record GetReporteInventarioPermanenteValorizadoQuery(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    string? FamiliaCode,
    string? FamiliaHastaCode,
    string? SubFamiliaCode,
    string? SubFamilia2Code,
    bool Resumen) : IRequest<ReporteInventarioPermanenteValorizadoResponse>;
