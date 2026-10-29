using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Incidencia detectada al cerrar un período (por planta y período). Se consulta desde la opción
/// de Cierre Mensual; el legacy las dejaba en ERRORES y se veían desde "Movimiento de Artículos".
/// </summary>
public class CierreIncidencia : IAuditableEntity
{
    public long Id { get; set; }

    public string PlantaCode { get; set; } = default!;
    public Planta Planta { get; set; } = default!;

    public string PeriodCode { get; set; } = default!;

    public string ArticuloCode { get; set; } = default!;
    public Articulo Articulo { get; set; } = default!;

    public TipoIncidenciaCierre Tipo { get; set; }

    /// <summary>Documento relacionado, si aplica.</summary>
    public string? DocumentoCode { get; set; }

    public string Mensaje { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
