// Cepheus.Domain/Logistica/Transacciones/GuiaDetalle.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Línea de detalle de una Guía de Remisión. NO mueve stock.
    ///
    /// Legacy: dbo.MGuiasDet, PK (Codigo_pla, Numero_gve, Codigo_art) — se
    /// CONSERVA esa PK: un artículo no puede repetirse en la misma guía (el
    /// PowerBuilder lo validaba con fg_buscacodigo("MGUIASARTICULOS")).
    /// Por eso ItemNumber NO es parte de la clave y puede renumerarse al
    /// eliminar una línea (el legacy lo hacía con Logi_sp_Actualiza_Item_Gui).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_pla   -> PlantaCode
    ///   Numero_gve   -> GuiaCode
    ///   Codigo_art   -> ArticuloCode (FK -> Articulo.Code)
    ///   Item_gve     -> ItemNumber (correlativo de línea, char(3) -> int)
    ///   Cantidad_gve -> Cantidad (decimal(12,2))
    ///   Codigo_Est   -> Estado (enum EstadoGuiaDetalle)
    ///   Activo_gui   -> IsVerified ('S'/'N' -> bool; verificación del artículo)
    ///
    /// Descripcion_Art y Codigo_Uni que el legacy mostraba en la línea se
    /// obtienen del Articulo relacionado (Name / UnidadMedidaCode).
    /// </summary>
    public class GuiaDetalle : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string GuiaCode { get; set; } = default!;
        public Guia Guia { get; set; } = default!;

        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        /// <summary>Correlativo de línea dentro de la guía (Item_gve).</summary>
        public int ItemNumber { get; set; }

        public decimal Cantidad { get; set; }

        public EstadoGuiaDetalle Estado { get; set; } = EstadoGuiaDetalle.Pendiente;

        /// <summary>Verificación del artículo (Activo_gui 'S'/'N').</summary>
        public bool IsVerified { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
