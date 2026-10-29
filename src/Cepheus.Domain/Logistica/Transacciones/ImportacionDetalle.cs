// Cepheus.Domain/Logistica/Transacciones/ImportacionDetalle.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Línea de artículo de una Importación. Legacy: dbo.MImportacionDet.
    /// PK legacy y actual: (Planta, Importación, Proveedor, Artículo).
    ///
    /// Cambios frente al legacy:
    ///   - Codigo_tdo -> ComprobantePagoCode FK Comunes.ComprobantePago (el comentario
    ///     legacy dice "Comprobante de pago"; mismo criterio que OrdenCompra)
    ///   - Codigo_Prv -> ProveedorCode FK Logistica.Maestros.Proveedor
    ///   - Numero_Doc -> NumeroDocumento; Fecha_Emi -> FechaEmision
    ///   - T_cambio -> TipoCambio: venta de Comunes.TipoCambio en FechaEmision (calculado)
    ///   - Valor_fob / Flete / Seguro -> ValorFob / Flete / Seguro (USD, decimal(12,4))
    ///   - Valor_Adu (columna calculada legacy) -> ValorAduana = ValorFob + Flete + Seguro
    ///   - Codigo_Est -> Estado (enum EstadoImportacion). Sigue siendo necesario:
    ///     la generación de ingreso es parcial por proveedor y marca cada línea.
    ///
    /// Campos de prorrateo (los escribe ImportacionProrrateoCalculator, equivalente
    /// a Logi_sp_Actualiza_Importacion; NO se digitan):
    ///   Porcentaje_Det -> PorcentajeDet, Valor_Det -> ValorDet (costo en soles),
    ///   Advalorem_Det -> AdvaloremDet, Sobretasa_Det -> SobretasaDet,
    ///   Igv_Det -> IgvDet, Otros_Gas_Det -> OtrosGastosDet
    /// </summary>
    public class ImportacionDetalle : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string ImportacionCode { get; set; } = default!;
        public Importacion Importacion { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        public string ComprobantePagoCode { get; set; } = default!;
        public ComprobantePago ComprobantePago { get; set; } = default!;

        public string NumeroDocumento { get; set; } = default!;
        public DateTime FechaEmision { get; set; }

        /// <summary>Tipo de cambio venta del día de FechaEmision. Calculado.</summary>
        public decimal TipoCambio { get; set; }

        public decimal Cantidad { get; set; }
        public decimal ValorFob { get; set; }
        public decimal Flete { get; set; }
        public decimal Seguro { get; set; }
        /// <summary>ValorFob + Flete + Seguro. Calculado.</summary>
        public decimal ValorAduana { get; set; }

        // Prorrateo (calculado)
        public decimal PorcentajeDet { get; set; }
        public decimal ValorDet { get; set; }
        public decimal AdvaloremDet { get; set; }
        public decimal SobretasaDet { get; set; }
        public decimal IgvDet { get; set; }
        public decimal OtrosGastosDet { get; set; }

        public EstadoImportacion Estado { get; set; } = EstadoImportacion.Pendiente;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
