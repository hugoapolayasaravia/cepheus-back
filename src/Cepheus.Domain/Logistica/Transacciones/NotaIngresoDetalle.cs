using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Línea de Nota de Ingreso.
///
/// Legacy: dbo.MNotaIngresoDet. La PK legacy es
/// (Codigo_Pla, Codigo_NoI, Codigo_Art, Codigo_Ped, Codigo_Gui); como Pedido y Guía son nulables
/// no pueden ser PK en EF, así que la PK es (PlantaCode, NotaIngresoCode, ItemNumber) (igual que
/// PedidoDetalle) y la combinación legacy queda como índice único.
///   Codigo_Art    -> ArticuloCode
///   Codigo_Ped    -> PedidoCode (pedido de origen; null si la línea de la OC es directa)
///   Codigo_Gui    -> GuiaCode (reservado para "Anexar Guías")
///   Item_Art      -> ItemNumber (correlativo 1..n dentro de la nota)
///   Cantidad_Art  -> Cantidad
///   Precio_Art    -> Precio
///   Descuento_Art -> Descuento: PORCENTAJE (0-100), igual que el legacy
///                    (Total = Cantidad × Precio × (1 − Descuento/100))
///   Total_Art     -> Total
///   Codigo_Est    -> Estado
/// </summary>
public class NotaIngresoDetalle : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public string NotaIngresoCode { get; set; } = default!;
    public NotaIngreso NotaIngreso { get; set; } = default!;

    public int ItemNumber { get; set; }

    public string ArticuloCode { get; set; } = default!;
    public Articulo Articulo { get; set; } = default!;

    public string? PedidoCode { get; set; }
    public Pedido? Pedido { get; set; }

    public string? GuiaCode { get; set; }

    

    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }

    public EstadoNotaIngresoDetalle Estado { get; set; } = EstadoNotaIngresoDetalle.Procesado;

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
