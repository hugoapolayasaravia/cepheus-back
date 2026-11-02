using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Tipo de pago de una cancelación (catálogo).
    ///
    /// Legacy: dbo.TTipPago. Datos del legacy: 01 Efectivo Oficina, 02 Cheque, 03 Letras, 04 Canje-Proveedor,
    /// 05 Cheque Gerencia, 07 Cheque Diferido, 08 Transferencia, 09 Depósito en Efectivo, 10 Canje N.Crédito,
    /// 11 Letras Varias, 12 Depósito en Cheque, 13 Pago de Detracción, 14 Canje-Letra.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tpa  -> Code (PK natural, char(2))
    ///   Descripcion -> Name
    ///   Orden_tpa   -> SortOrder
    ///   Visible_apr -> ShowInApproval
    ///   Cuenta_sol / Cuenta_dol -> NO se migran ("NO VA"): las cuentas contables del tipo de pago se
    ///                              configuran por moneda en TipoPagoCuentaContable (módulo Cancelaciones).
    /// </summary>
    public class TipoPago : IAuditableEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int SortOrder { get; set; }
        public bool ShowInApproval { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>Cuentas contables del tipo de pago, una por moneda.</summary>
        public ICollection<TipoPagoCuentaContable> Accounts { get; set; } = new List<TipoPagoCuentaContable>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
