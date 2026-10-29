using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Nota concreta de una Cotizacion (texto libre, típicamente iniciado a
    /// partir de una NotaCotizacionPlantilla del negocio pero editable por
    /// cotización). Hijo del aggregate Cotizacion.
    ///
    /// Legacy: dbo.CotizacionNotas (SQL Server). PK compuesta
    /// (Codigo_Neg, Ano_cot, Mes_cot, Codigo_cot, Clave), donde Clave es
    /// IDENTITY(1,1) NOT FOR REPLICATION.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Neg, Ano_cot, Mes_cot, Codigo_cot -> FK compuesta -> Cotizacion
    ///   Clave           -> Sequence (identity, parte de la PK)
    ///   Descripcion_not -> Description (text -> string)
    ///   Opcion_not      -> Option (enum OpcionNotaCotizacion, reutilizado de
    ///                      NotaCotizacionPlantilla.Option)
    /// </summary>
    public class CotizacionNota : IAuditableEntity
    {
        public string NegocioCode { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public string Code { get; set; } = default!;
        public Cotizacion Cotizacion { get; set; } = default!;

        /// <summary>Correlativo autogenerado (IDENTITY en BD).</summary>
        public int Sequence { get; set; }

        public string Description { get; set; } = default!;

        public OpcionNotaCotizacion Option { get; set; } = OpcionNotaCotizacion.Observacion;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
