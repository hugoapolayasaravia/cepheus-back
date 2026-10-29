using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Stock de un artículo en una planta específica. Entidad crítica de
    /// concurrencia: se actualizará constantemente desde el futuro módulo de
    /// Transacciones (entradas/salidas de almacén), por lo que NUNCA se
    /// expone una operación que sobrescriba <see cref="Quantity"/>
    /// directamente — solo incrementos/decrementos controlados.
    ///
    /// Legacy: dbo.Stock_Plt_Art (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Pla   -> PlantaCode (FK -> Comunes.Planta.Code; mismo patrón
    ///                   confirmado en Codigo_Pla de ArticuloProveedor)
    ///   Codigo_Art   -> ArticuloCode (FK -> Articulo.Code)
    ///   Stock        -> Quantity
    ///   Costo_Uni    -> UnitCost (float legacy -> decimal(18,4); float no es
    ///                   apropiado para montos, se corrige el tipo)
    ///   Costo_UnD    -> UnitCostUsd (mismo criterio de tipo)
    ///   Precio_Pro   -> AverageCost (nullable, mismo criterio de tipo)
    ///   Stock_Min    -> MinStock
    ///   Stock_Max    -> MaxStock
    ///
    /// El legacy no tiene columna de concurrencia ni auditoría. Se agrega
    /// RowVersion — es obligatorio acá, no opcional como en otras entidades,
    /// por el requisito explícito de controlar concurrencia y stock negativo.
    /// </summary>
    public class ArticuloStock : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        public decimal Quantity { get; set; }

        public decimal UnitCost { get; set; }
        public decimal UnitCostUsd { get; set; }
        public decimal? AverageCost { get; set; }

        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista — crítico para esta entidad.
        public byte[] RowVersion { get; set; } = default!;
    }
}
