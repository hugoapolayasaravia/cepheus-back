using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Logistica.Catalogos;

namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Línea de un Documento de Compra. Legacy: dbo.DDocCompras.
    /// PK: (ComprobantePagoCode, NumeroDocumento, ProveedorCode, ItemNumber).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tdo / Numero_dco / Codigo_prv -> clave del DocumentoCompra
    ///   Item_dco     -> ItemNumber
    ///   Codigo_pla   -> PlantaCode (FK Comunes.Planta)
    ///   codigo_cta   -> CuentaContableCode (FK Contabilidad.CuentaContable)
    ///   Codigo_tip + Codigo_ane -> AnexoTipoCode + AnexoCode (FK compuesta a Anexo; opcional)
    ///   Detalle_dco  -> Detalle (nvarchar(50))
    ///   Cantidad_dco -> Cantidad
    ///   Codigo_uni   -> UnidadMedidaCode (FK Logistica.UnidadMedida, char(2): la unidad de medida de COMPRAS,
    ///                   la misma del artículo de la nota de ingreso; no es la unidad de venta de Facturación)
    ///   PrecioS_dco  -> PrecioSol
    ///   ParcialS_dco -> ParcialSol = Cantidad x PrecioSol
    ///   Numero_gve   -> NumeroGuiaVenta (sin FK: no existe aún la guía de venta en Cepheus)
    ///   Codigo_prd   -> ProductoTipoCode (2) + ProductoCode (4): FK compuesta a Producto de
    ///                   Facturación (el legacy guardaba el código concatenado de 6); opcional.
    /// </summary>
    public class DocumentoCompraDetalle : IAuditableEntity
    {
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public DocumentoCompra DocumentoCompra { get; set; } = default!;

        public int ItemNumber { get; set; }

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string CuentaContableCode { get; set; } = default!;
        public CuentaContable CuentaContable { get; set; } = default!;

        public string? AnexoTipoCode { get; set; }
        public string? AnexoCode { get; set; }
        public Anexo? Anexo { get; set; }

        public string Detalle { get; set; } = default!;

        public decimal Cantidad { get; set; }

        public string UnidadMedidaCode { get; set; } = default!;
        public UnidadMedida UnidadMedida { get; set; } = default!;

        public decimal PrecioSol { get; set; }
        public decimal ParcialSol { get; set; }

        public string? NumeroGuiaVenta { get; set; }

        public string? ProductoTipoCode { get; set; }
        public string? ProductoCode { get; set; }
        public Producto? Producto { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
