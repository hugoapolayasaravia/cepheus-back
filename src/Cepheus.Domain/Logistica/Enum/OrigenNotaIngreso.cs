namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Origen de la Nota de Ingreso. Legacy: MNotaIngresoRes.Origen_Noi
/// (C = Compra, I = Importaciones, T = Transferencia).
/// Importacion y Transferencia (Vale de Salida) quedan reservados.
/// </summary>
public enum OrigenNotaIngreso
{
    Compra,
    Importacion,
    Transferencia
}
