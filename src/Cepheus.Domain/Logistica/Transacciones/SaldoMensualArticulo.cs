using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Saldo valorizado de un artículo al cierre de un período (una fila por planta, período y artículo).
///
/// Reemplaza a dbo.MCierreMes del legacy, que tenía dos columnas por mes (D200306S / D200306P ...),
/// obligaba a agregar columnas cada año y a armar SQL dinámico y funciones con un IF por mes
/// (fn_cargacierre). Aquí cada mes es una fila: no hay límite de períodos ni SQL dinámico.
///
/// Equivalencias:
///   D{AAAAMM}S -> Quantity      (saldo en cantidad)
///   D{AAAAMM}P -> AverageCost   (costo promedio, 6 decimales como el legacy)
///   S * P      -> importe valorizado: no se guarda; se calcula como ROUND(Quantity * AverageCost, 2)
///                 (KardexValorizador.Importe), así no depende de columnas calculadas propias de un motor.
///
/// Solo lo escribe el cierre mensual, en la misma transacción que registra el ControlCierre.
/// </summary>
public class SaldoMensualArticulo : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public Planta Planta { get; set; } = default!;

    /// <summary>Período AAAAMM.</summary>
    public string PeriodCode { get; set; } = default!;

    public string ArticuloCode { get; set; } = default!;
    public Articulo Articulo { get; set; } = default!;

    public decimal Quantity { get; set; }
    public decimal AverageCost { get; set; }

    /// <summary>Total ingresado durante el período (informativo, para conciliar).</summary>
    public decimal IncomingQuantity { get; set; }

    /// <summary>Total retirado durante el período (informativo, para conciliar).</summary>
    public decimal OutgoingQuantity { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
