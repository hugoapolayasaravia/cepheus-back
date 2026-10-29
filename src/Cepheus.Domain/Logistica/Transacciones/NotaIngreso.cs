using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Nota de Ingreso (cabecera). Cierra el ciclo de ingreso de un artículo:
/// Pedido -> Cotización -> Orden de Compra -> Nota de Ingreso.
///
/// Legacy: dbo.MNotaIngresoRes (SQL Server).
/// Mapeo de columnas legacy -> propiedades:
///   Codigo_Pla        -> PlantaCode (FK Comunes.Planta)
///   Codigo_NoI        -> Code (correlativo por planta, 9XXXXX)
///   Condicion_NoI     -> Condicion (enum)
///   Codigo_com        -> OrdenCompraCode (FK compuesta Planta+OC; null en Nota de Crédito)
///   Codigo_tdo        -> ComprobantePagoId (FK Comunes.ComprobantePago, solo AvailableForPurchaseOrder)
///   Codigo_Mot        -> MotivoDevolucionId (FK Comunes.MotivoDevolucion; solo Nota de Crédito)
///   Numero_Doc        -> NumeroDocumento
///   Numero_Gui        -> NumeroGuia
///   Numero_Fac        -> NumeroReferencia (serie-número del documento referenciado, Nota de Crédito)
///   Origen_Noi        -> Origen (enum)
///   Codigo_td1        -> ComprobantePagoReferenciaId
///   Codigo_Prv        -> ProveedorCode
///   Moneda            -> MonedaCode (Comunes.Moneda, ISO)
///   Fecha_ent         -> FechaRecepcion
///   Codigo_pgc        -> FormaPagoCode
///   T_Cambio          -> TipoCambio
///   Fecha_Pro         -> FechaProceso (momento en que se crea la nota)
///   Codigo_Est        -> Estado (enum, ver EstadoNotaIngreso)
///   Fecha_Emi         -> FechaEmision (del comprobante de pago)
///   Igv_Noi           -> Igv
///   Total_Noi         -> Total (suma de las líneas)
///   Monto_Noi         -> Monto (importe final tras IGV / renta / etc.)
///   NoGravableSol_dco -> NoGravable
///   RentaSol_dco      -> Renta
///   FonaviSol_dco     -> Fonavi
///   ServicioSol_dco   -> Servicio
///   IgvExtSol_dco     -> IgvExterior
///   Asiento_noi       -> AsientoContable (dato de contabilidad, se mantiene)
///   Usuario           -> CreatedBy (auditoría)
///   Codigo_Val        -> CodigoVale (futuro vale de salida, se mantiene)
///   Fecha_Doc         -> FechaDocumento (fecha del comprobante de pago)
/// </summary>
public class NotaIngreso : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public Planta Planta { get; set; } = default!;

    public string Code { get; set; } = default!;

    public CondicionNotaIngreso Condicion { get; set; } = CondicionNotaIngreso.OrdenCompra;
    public OrigenNotaIngreso Origen { get; set; } = OrigenNotaIngreso.Compra;

    public string? OrdenCompraCode { get; set; }
    /// <summary>Importación de origen (Origen = Importacion). Legacy: Codigo_com con Origen_Noi = 'I'.</summary>
    public string? ImportacionCode { get; set; }
    public Importacion? Importacion { get; set; }
    public OrdenCompra? OrdenCompra { get; set; }

    public string ComprobantePagoCode { get; set; } = default!;
    public ComprobantePago ComprobantePago { get; set; } = default!;

    public string? MotivoDevolucionCode { get; set; }
    public MotivoDevolucionArticulo? MotivoDevolucion { get; set; }

    public string? NumeroDocumento { get; set; }
    public string? NumeroGuia { get; set; }
    public string? NumeroReferencia { get; set; }

    public string? ComprobantePagoReferenciaCode { get; set; }
    public ComprobantePago? ComprobantePagoReferencia { get; set; }

    public string ProveedorCode { get; set; } = default!;
    public Proveedor Proveedor { get; set; } = default!;

    public string MonedaCode { get; set; } = default!;
    public Moneda Moneda { get; set; } = default!;

    public string FormaPagoCode { get; set; } = default!;
    public FormaPago FormaPago { get; set; } = default!;

    public DateTime FechaRecepcion { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime FechaProceso { get; set; }
    public DateTime? FechaDocumento { get; set; }

    /// <summary>Solo se usa cuando el comprobante es en dólares (0 en soles).</summary>
    public decimal TipoCambio { get; set; }

    public EstadoNotaIngreso Estado { get; set; } = EstadoNotaIngreso.Procesado;

    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal Monto { get; set; }
    public decimal NoGravable { get; set; }
    public decimal Renta { get; set; }
    public decimal Fonavi { get; set; }
    public decimal Servicio { get; set; }
    public decimal IgvExterior { get; set; }

    public string? AsientoContable { get; set; }
    public string? CodigoVale { get; set; }

    // ---- Transferencia entre almacenes (Origen = Transferencia) ----
    // El ingreso se genera en el almacén DESTINO a partir de un Vale de Salida tipo Transferencia (005) del almacén
    // ORIGEN. En el legacy el código del vale se guardaba en Codigo_com (el campo "Orden de compra") y el origen
    // iba dentro de Numero_doc ('TRANSFERENCIA' + planta origen). Aquí son campos propios con clave foránea al vale:
    // OrdenCompraCode es una clave foránea a la orden de compra y no puede guardar un código de vale.

    /// <summary>Planta que envía (dueña del vale). Null si la nota no es una transferencia.</summary>
    public string? PlantaOrigenCode { get; set; }

    /// <summary>
    /// Código del vale de transferencia en la planta origen. Un vale se recibe una sola vez mientras su nota no esté
    /// anulada; al anular la transferencia el vale vuelve a Pendiente.
    /// </summary>
    public string? ValeTransferenciaCode { get; set; }

    public Vale? ValeTransferencia { get; set; }

    public ICollection<NotaIngresoDetalle> Detalles { get; set; } = new List<NotaIngresoDetalle>();

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
