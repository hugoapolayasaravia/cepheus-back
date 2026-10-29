namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;

/// <summary>
/// Línea enviada al registrar / agregar a una Orden de Servicio. El total, el correlativo de ítem, la
/// unidad de medida y la planta afectada los calcula el servidor.
/// Descuento es un PORCENTAJE (0-100), igual que el legacy.
///
/// Reglas por tipo de vale (PowerBuilder w_log_ordenservicio, dw_5):
///   COT -> la Orden de Trabajo es obligatoria (debe estar en ejecución) y, si no se envían, el sub centro
///          de costo y el sub centro ejecutor se toman de la OT.
///   TRA -> el sub centro de costo debe ser uno de los permitidos para transferencias.
///   Otros tipos -> no llevan Orden de Trabajo ni sub centro ejecutor.
/// </summary>
public sealed class OrdenServicioDetalleRequest
{
    public string ArticuloCode { get; set; } = default!;
    public string? Glosa { get; set; }
    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Descuento { get; set; }
    public string TipoValeCode { get; set; } = default!;
    public string? SubCentroCostoCode { get; set; }
    public string? SubCentroEjecutorCode { get; set; }
    public string? OrdenTrabajoCode { get; set; }
}
