namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Tipo de ajuste de una línea. Legacy: dbo.MAjustesInventarioDet.Tipo_Aju
/// (I = Sobrantes, S = Faltantes). Se persiste como ordinal (mismo criterio que OrigenNotaIngreso).
///
///   Sobrante = 0 (I): hay más en el almacén que en el sistema  => suma stock al procesar.
///   Faltante = 1 (S): hay menos en el almacén que en el sistema => resta stock al procesar.
/// </summary>
public enum TipoAjusteInventario
{
    Sobrante = 0,
    Faltante = 1
}
