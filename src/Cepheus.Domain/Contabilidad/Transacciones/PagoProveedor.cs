using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Catalogos;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Pago a Proveedor (cancelación de documentos de compra con cheque, transferencia o depósito). Cabecera.
    ///
    /// Legacy: dbo.RCheCompras (SQL Server). Todo en soles: no se migran los campos en dólares
    /// ni el tipo de cambio / diferencia de cambio (indicación del usuario).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_che               -> Code (PK, correlativo de 10 caracteres)
    ///   NumCheque_che            -> NumeroCheque (ingreso manual)
    ///   Codigo_ban + numero_cta  -> BancoCode + CuentaBancariaNumero (FK compuesta a Contabilidad.CuentaBancaria)
    ///   Codigo_prv               -> ProveedorCode (FK Logistica.Proveedor)
    ///   Numero_com               -> NumeroComprobanteRetencion (opcional hasta que se registra)
    ///   FecEmision_che           -> FechaEmision
    ///   Moneda_che               -> MonedaCode (FK Comunes.Moneda; solo soles)
    ///   PagoSol_che              -> PagoSol
    ///   RetencionSol_che         -> RetencionSol
    ///   TotalSol_che             -> TotalSol (PagoSol - RetencionSol)
    ///   AsiCancelacion_che       -> AsientoCancelacionCode (sin FK: se genera después)
    ///   AsiAnulacion_che         -> AsientoAnulacionCode (sin FK)
    ///   Estado_che               -> Estado (enum EstadoPagoProveedor)
    ///   fecAnulacion_che         -> FechaAnulacion
    ///   impresion_che            -> ComprobanteCajaBancosImpreso (S/N -> bool)
    ///   afecto_che               -> Afecto (S/N -> bool; resultado de fg_afecto)
    ///   NroControl_che           -> NumeroControl (char(1), se conserva tal cual)
    ///   glosa_che                -> Glosa
    ///   fecomreten_che           -> FechaComprobanteRetencion
    ///   pagarprv_che             -> PagarA
    ///   codigo_pla               -> PlantaCode (FK Comunes.Planta)
    ///   ImpRetencion_che         -> ComprobanteRetencionImpreso (S/N -> bool)
    ///   detraccion_dco           -> Detraccion (S/N -> bool)
    ///   codigo_cta               -> CuentaContableCode (FK Contabilidad.CuentaContable)
    ///   Canje_che                -> Canje (S/N -> bool)
    ///   MonReferencial_che       -> MontoReferencial
    ///   PagoSolV_che             -> PagoSolVenta (parte de venta en cheques mixtos)
    ///   Codigo_cli               -> ClienteCode (opcional; clientes y mixtos)
    ///   PagSolReal_che           -> PagoRealSol (monto realmente pagado)
    ///   tipo_che                 -> Tipo (enum TipoPagoProveedor)
    ///   COD_FOR_PAG_CANCELACION  -> FormaPagoCancelacionCode (FK Contabilidad.FormaPagoCancelacion)
    ///   NUM_ITEM                 -> ProveedorCuentaId (FK Logistica.ProveedorCuenta; reemplaza COM_CTAS_PROVEEDORES)
    ///   IND_DEP_ELECTRONICO      -> DepositoElectronico (1/0 -> bool)
    ///   Codigo_usu / FecProceso_che -> cubiertos por CreatedBy / CreatedAt (auditoría)
    ///
    /// Se eliminan: tipcambio_che, PagoDol_che, RetencionDol_che, TotalDol_che, PagoDolV_che,
    /// totdifsol_che (todo lo relacionado con dólares y diferencia de cambio).
    /// </summary>
    public class PagoProveedor : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string NumeroCheque { get; set; } = default!;
        public string NumeroControl { get; set; } = "*";

        public string BancoCode { get; set; } = default!;
        public Banco Banco { get; set; } = default!;

        public string CuentaBancariaNumero { get; set; } = default!;
        public CuentaBancaria CuentaBancaria { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string? NumeroComprobanteRetencion { get; set; }
        public DateTime? FechaComprobanteRetencion { get; set; }

        public DateTime FechaEmision { get; set; }

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public decimal PagoSol { get; set; }
        public decimal RetencionSol { get; set; }
        public decimal TotalSol { get; set; }
        public decimal PagoRealSol { get; set; }
        public decimal PagoSolVenta { get; set; }
        public decimal MontoReferencial { get; set; }

        public string? AsientoCancelacionCode { get; set; }
        public string? AsientoAnulacionCode { get; set; }

        public EstadoPagoProveedor Estado { get; set; } = EstadoPagoProveedor.Pendiente;
        public DateTime? FechaAnulacion { get; set; }

        public bool ComprobanteCajaBancosImpreso { get; set; }
        public bool ComprobanteRetencionImpreso { get; set; }
        public bool Afecto { get; set; } = true;
        public bool Detraccion { get; set; }
        public bool Canje { get; set; }
        public bool DepositoElectronico { get; set; }

        public string Glosa { get; set; } = default!;
        public string PagarA { get; set; } = default!;

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string CuentaContableCode { get; set; } = default!;
        public CuentaContable CuentaContable { get; set; } = default!;

        public string? ClienteCode { get; set; }
        public Cliente? Cliente { get; set; }

        public TipoPagoProveedor Tipo { get; set; } = TipoPagoProveedor.Proveedores;

        public string? FormaPagoCancelacionCode { get; set; }
        public FormaPagoCancelacion? FormaPagoCancelacion { get; set; }

        public int? ProveedorCuentaId { get; set; }
        public ProveedorCuenta? ProveedorCuenta { get; set; }

        public ICollection<PagoProveedorDetalle> Detalles { get; set; } = new List<PagoProveedorDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
