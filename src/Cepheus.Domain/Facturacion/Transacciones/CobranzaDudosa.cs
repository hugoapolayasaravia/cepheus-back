using Cepheus.Domain.Comun;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Documento de venta enviado a cobranza dudosa (legacy: dbo.MCobranzaDudosa, ventana w_con_cobranzadudosa).
    ///
    /// Clave: el legacy usaba (Codigo_tdo, Numero_doc), que no distingue la planta de las notas y letras ni el
    /// cliente y banco de los cheques. Aquí es (TypeCode, PlantaCode, Number, ClienteCode, BancoCode): PlantaCode va
    /// vacío salvo en NC (07), ND (08) y letras (10); BancoCode va vacío salvo en cheques (11).
    ///
    /// Legacy -> aquí:
    ///   Codigo_tdo / Numero_doc  -> TypeCode / Number
    ///   Codigo_cli / Codigo_obr  -> ClienteCode / ObraCode
    ///   Codigo_Ban               -> BancoCode
    ///   Fecha_pro / Fecha_sis    -> ProcessDate / SystemDate
    ///   Tipo_con                 -> TipoContabilidad
    ///   AsiProvision_doc         -> ProvisionEntry (asiento de provisión original del documento)
    ///   AsiCanje_doc             -> CanjeEntry (asiento de canje a cobranza dudosa; null hasta generarlo)
    ///   TotalSol_doc/TotalDol_doc, TipoCambio_doc, Moneda_doc -> TotalSoles, TotalDollars, ExchangeRate, CurrencyCode
    ///   Estado ('01'/'XX')       -> Status
    ///   Fecha_Cas                -> WrittenOffAt (castigo)
    /// </summary>
    public class CobranzaDudosa : IAuditableEntity
    {
        public string TypeCode { get; set; } = default!;
        public string PlantaCode { get; set; } = string.Empty;
        public string Number { get; set; } = default!;
        public string ClienteCode { get; set; } = default!;
        public string BancoCode { get; set; } = string.Empty;

        public string? ObraCode { get; set; }

        public DateTime ProcessDate { get; set; }
        public DateTime SystemDate { get; set; }

        public TipoContabilidad TipoContabilidad { get; set; } = TipoContabilidad.Todos;

        public string CurrencyCode { get; set; } = default!;

        /// <summary>Tipo de cambio venta de la fecha de proceso.</summary>
        public decimal ExchangeRate { get; set; }
        public decimal TotalSoles { get; set; }
        public decimal TotalDollars { get; set; }

        public string? ProvisionEntry { get; set; }
        public string? CanjeEntry { get; set; }

        public EstadoRegistroCobranzaDudosa Status { get; set; } = EstadoRegistroCobranzaDudosa.Registrado;
        public DateTime? WrittenOffAt { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
