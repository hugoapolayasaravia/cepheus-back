using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Forma de pago para ventas (catálogo simple, con plazo y condición de
    /// crédito). Usado en el módulo de Facturación (Cliente.FormaPagoVentaCode y
    /// Obra.FormaPagoVentaCode).
    ///
    /// Legacy: no se entregó el DDL de esta tabla. Solo se conocen las columnas
    /// MClientes.CodigoPgv_cli char(2) NOT NULL y MObras.CodigoPgv_obr char(2)
    /// NULL. Se modela con la misma forma que Logística.FormaPago
    /// (dbo.TForPagCompras, que ya trae la columna credito_pgv), por analogía.
    /// Mapeo (supuesto):
    ///   CodigoPgv       -> Code (PK natural, char(2), correlativo)
    ///   (descripción)   -> Name (longitud asumida: 30)
    ///   (días)          -> Days (plazo en días)
    ///   (crédito S/N)   -> IsCredit (bool, default false)
    ///
    /// IsActive no existe en el legacy; se agrega por consistencia con el resto
    /// de catálogos del sistema.
    /// </summary>
    public class FormaPagoVenta : IAuditableEntity
    {
        /// <summary>
        /// Código de la forma de pago de ventas (PK natural, 2 caracteres, correlativo).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        /// <summary>
        /// Plazo de pago, en días.
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Indica si la forma de pago es al crédito. Default: false.
        /// </summary>
        public bool IsCredit { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
