// Cepheus.Domain/Logistica/Transacciones/Importacion.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Importación (cabecera / póliza). Legacy: dbo.MImportacionRes.
    /// Proceso aparte del flujo Pedido -> Cotización -> Orden de Compra -> Nota de Ingreso.
    ///
    /// Cambios frente al legacy:
    ///   - Codigo_Pla -> PlantaCode FK Comunes.Planta
    ///   - Codigo_Imp -> Code: correlativo de 6 dígitos por planta
    ///   - peso_net / peso_bru -> PesoNeto / PesoBruto
    ///   - fecha_pro y Codigo_usu -> cubiertos por CreatedAt / CreatedBy
    ///   - fecha_Pol -> FechaPoliza (manual); fecha_Ent -> FechaEntrega (fecha de recepción, manual)
    ///   - T_cambio -> TipoCambio: venta de Comunes.TipoCambio en FechaPoliza (no se digita)
    ///   - Codigo_est (char(2)) -> Estado (enum EstadoImportacion)
    ///   - T_valor_fob / T_flete / T_seguro: suma del detalle (ImportacionTotalsCalculator)
    ///   - T_valor_Adu (columna calculada legacy) -> TotalAduana = Fob + Flete + Seguro
    ///   - Advalorem / Sobretasa / Otros_Gas: digitados por el usuario
    ///   - Igv_Imp -> Igv: round((TotalAduana + Advalorem + Sobretasa) x IGV% / 100, 2)
    /// </summary>
    public class Importacion : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        /// <summary>Correlativo por planta, 6 dígitos (Codigo_Imp, char(6)).</summary>
        public string Code { get; set; } = default!;

        public decimal PesoNeto { get; set; }
        public decimal PesoBruto { get; set; }

        public DateTime FechaPoliza { get; set; }
        public DateTime? FechaEntrega { get; set; }

        /// <summary>Tipo de cambio venta del día de FechaPoliza. Calculado.</summary>
        public decimal TipoCambio { get; set; }

        public decimal TotalFob { get; set; }
        public decimal TotalFlete { get; set; }
        public decimal TotalSeguro { get; set; }
        public decimal TotalAduana { get; set; }

        /// <summary>Digitado por el usuario.</summary>
        public decimal Advalorem { get; set; }
        /// <summary>Digitado por el usuario.</summary>
        public decimal Sobretasa { get; set; }
        /// <summary>Calculado.</summary>
        public decimal Igv { get; set; }
        /// <summary>Digitado por el usuario.</summary>
        public decimal OtrosGastos { get; set; }

        public EstadoImportacion Estado { get; set; } = EstadoImportacion.Pendiente;

        public ICollection<ImportacionDetalle> Detalles { get; set; } = new List<ImportacionDetalle>();
        public ICollection<ImportacionGasto> Gastos { get; set; } = new List<ImportacionGasto>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
