namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Condición de la Nota de Ingreso. Legacy: MNotaIngresoRes.Condicion_NoI
/// (O = Orden de Compra, A = Anexar Guías).
/// AnexarGuias queda reservado: su flujo (MGuiasIngreso) aún no está migrado.
/// </summary>
public enum CondicionNotaIngreso
{
    OrdenCompra,
    AnexarGuias
}
