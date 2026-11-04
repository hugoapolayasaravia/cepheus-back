using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Letra de un cliente. Nace en el módulo de Cancelaciones (tipo de pago 03 Letras) o al refinanciar documentos
    /// con "Letras Varias" (tipo 11); aquí se consulta, se corrige (fechas, aceptación, glosa) y se protesta.
    ///
    /// Legacy: dbo.RLetVentas. PK legacy (Numero_let) -> PK compuesta (PlantaCode, Number): la numeración pertenece
    /// a la planta (Planta.NumeroLet, formato "000-00000").
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_pla       -> PlantaCode
    ///   Numero_let       -> Number
    ///   Codigo_cli       -> ClienteCode
    ///   codigo_ban       -> BancoCode (opcional: el legacy guarda blancos si no hay banco)
    ///   FecEmision_let   -> IssueDate
    ///   FecPago_let      -> DueDate (vencimiento)
    ///   Codigo_cob       -> CobradorCode
    ///   TipCambio_let    -> ExchangeRate
    ///   moneda_let       -> MonedaCode (S/D -> PEN/USD)
    ///   TotNetoSol_let   -> TotalAmount (una sola moneda + tipo de cambio)
    ///   PendienteSol_let -> PendingAmount
    ///   AsiProvision_let -> ProvisionEntryNumber (asiento de la cancelación que la originó)
    ///   AsiAnulacion_let -> CancellationEntryNumber, FecAnulacion_let -> CanceledAt
    ///   glosa_let        -> Description
    ///   codigo_can       -> CancelacionCode
    ///   impresion_let    -> IsPrinted (S/N)
    ///   FecCancelacion_let -> SettledAt
    ///   Interes          -> Interest
    ///   Acepta_Cli       -> AcceptedByClient (S/N)
    ///   Estado           -> Status (enum EstadoLetra)
    ///   Clave, FechaFinal_val, CobranzaDudosa, TipCamCdu, FecProceso_let -> no se migran / auditoría
    /// </summary>
    public class LetraVenta : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string Number { get; set; } = default!;

        public string ClienteCode { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;

        public string? BancoCode { get; set; }
        public Banco? Banco { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }

        public string CobradorCode { get; set; } = default!;
        public Cobrador Cobrador { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;
        public decimal ExchangeRate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }

        /// <summary>Cobranza dudosa (legacy CobranzaDudosa S/N/X): Normal, Dudosa (enviado a cobranza dudosa) o Castigada.</summary>
        public EstadoCobranzaDudosa DoubtfulStatus { get; set; } = EstadoCobranzaDudosa.Normal;

        /// <summary>Tipo de cambio con que se registró la cobranza dudosa (legacy TipCamCdu).</summary>
        public decimal DoubtfulExchangeRate { get; set; }
        public decimal Interest { get; set; }

        public string? ProvisionEntryNumber { get; set; }
        public string? CancellationEntryNumber { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime? SettledAt { get; set; }

        public string? Description { get; set; }

        /// <summary>Cancelación que originó la letra (sus documentos son los que la letra canjea).</summary>
        public string CancelacionCode { get; set; } = default!;
        public CancelacionVenta Cancelacion { get; set; } = default!;

        public bool IsPrinted { get; set; }
        public bool AcceptedByClient { get; set; }

        public EstadoLetra Status { get; set; } = EstadoLetra.Facturado;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
