namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Documento o proceso que originó un movimiento de artículo.
/// Se persiste con el valor numérico (mismo criterio que los demás enums del módulo).
///
/// Equivalencia con el ORIGEN del cursor de Logi_sp_Actualiza_Mes_Consumos:
///   NotaIngreso   -> 'NI' (MNotaIngresoRes/Det, compra)
///   NotaCredito   -> 'NI' con Codigo_tdo = '07' (devolución, motivo 02)
///   Importacion   -> 'NI' generada desde una importación
///   OrdenServicio -> 'OS'
///   ValeSalida    -> 'VS' (MValesRes/Det)
///   AjusteIngreso -> 'AI' (MAjustesInventario, Tipo_Aju = 'I')
///   AjusteSalida  -> 'AS' (MAjustesInventario, Tipo_Aju = 'S')
///
/// Apertura y AjusteManual no existían en el legacy:
///   Apertura     = saldo con el que arranca el Kardex de un artículo (carga inicial o alta del stock).
///   AjusteManual = incremento/decremento directo desde el maestro de stock, sin documento.
/// </summary>
public enum OrigenMovimientoArticulo
{
    Apertura = 0,
    NotaIngreso = 1,
    NotaCredito = 2,
    Importacion = 3,
    OrdenServicio = 4,
    ValeSalida = 5,
    AjusteIngreso = 6,
    AjusteSalida = 7,
    AjusteManual = 8,

    /// <summary>
    /// Guía de Ingreso (legacy 'GI', MGuiasIngreso). Reservado: el flujo "Anexar Guías" aún no está migrado;
    /// cuando lo esté, sus handlers deben registrar el movimiento con este origen.
    /// </summary>
    GuiaIngreso = 9,

    /// <summary>
    /// Evento de valorización SIN movimiento de stock: Nota de Crédito por ajuste de precio (motivo 01).
    /// Solo existe en memoria durante el cálculo del cierre; nunca se guarda en MovimientosArticulo.
    /// </summary>
    AjustePrecio = 10,

    /// <summary>
    /// Ingreso en el almacén destino de una transferencia entre almacenes (Nota de Ingreso de origen Transferencia).
    /// La salida en el origen se registra como ValeSalida (el vale de transferencia). En el Kardex legacy ambas
    /// aparecen como 'NI' en el destino y 'VS' en el origen.
    /// </summary>
    TransferenciaIngreso = 11
}
