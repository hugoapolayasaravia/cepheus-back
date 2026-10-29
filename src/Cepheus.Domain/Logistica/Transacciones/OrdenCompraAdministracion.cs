// Cepheus.Domain/Logistica/Transacciones/OrdenCompraAdministracion.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Orden de Compra de Administración: orden para recibos por honorarios y
    /// gastos del área administrativa. NO maneja artículos ni mueve stock (a
    /// diferencia de OrdenCompra, que viene de dbo.MComprasRes). Es la "Orden de
    /// Compra Administración" a la que apunta RDocCompras.Codigo_Com.
    ///
    /// Legacy: dbo.MCompraR (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Com        -> Code (PK natural, char(6), correlativo)
    ///   Codigo_tdo        -> ComprobantePagoCode (FK Comunes.ComprobantePago)
    ///   Emitir_Com        -> EmitirDocumento (varchar(30))
    ///   Fecha_Ent         -> FechaEntrega
    ///   Codigo_Prv        -> ProveedorCode (FK Logistica.Proveedor)
    ///   Codigo_Cop        -> CompradorCode (FK Comprador)
    ///   Moneda (char(1))  -> MonedaCode (FK Comunes.Moneda, 3 chars; mismo criterio que OrdenCompra)
    ///   Codigo_Env        -> LugarEnvioCode (FK LugarEnvio)
    ///   Codigo_pgc        -> FormaPagoCode (FK FormaPago)
    ///   Codigo_Trm        -> TramiteCode (FK Tramite)
    ///   Observacion_Com / Observacion_Co1 -> Observaciones1 / Observaciones2
    ///   Codigo_Not        -> NotaCompraCode (FK NotaCompra, opcional)
    ///   Codigo_Mot        -> Motivo (enum MotivoOrdenCompraAdministracion)
    ///   Codigo_Est        -> Estado (enum EstadoOrdenCompraAdministracion)
    ///   Usuario / Fecha_Pro -> cubiertos por CreatedBy / CreatedAt
    ///   Usuario_Apro / Fecha_Apr -> AprobadoPor / FechaAprobacion
    ///   Total_Com (legacy) -> NetoCompra, Monto_Com (legacy) -> TotalCompra
    ///     (mismo criterio que OrdenCompra: los nombres legacy estaban cambiados)
    ///   Igv_Com           -> IgvCompra
    ///   NoGravableSol_dco / RentaSol_dco / FonaviSol_dco / ServicioSol_dco / IgvExtSol_dco
    ///                     -> NoGravableCompra / RentaCompra / FonaviCompra / ServicioCompra / IgvExteriorCompra
    ///
    /// Columna nueva (no existe en el legacy): UnidadNegocioCode, necesaria para
    /// resolver la matriz de aprobación (RangoAprobacion se define por unidad de negocio).
    ///
    /// Totales: NetoCompra/IgvCompra/RentaCompra/FonaviCompra se calculan siempre desde
    /// el detalle + ComprobantePago + ControlVentas (ver OrdenCompraAdministracionTotalsCalculator).
    /// NoGravableCompra/ServicioCompra/IgvExteriorCompra son digitados por el usuario.
    /// </summary>
    public class OrdenCompraAdministracion : IAuditableEntity
    {
        /// <summary>Correlativo (PK natural, char(6)).</summary>
        public string Code { get; set; } = default!;

        public string ComprobantePagoCode { get; set; } = default!;
        public ComprobantePago ComprobantePago { get; set; } = default!;

        /// <summary>Legacy Emitir_Com (varchar(30)): texto para emitir el documento.</summary>
        public string? EmitirDocumento { get; set; }

        public DateTime FechaEntrega { get; set; }

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string CompradorCode { get; set; } = default!;
        public Comprador Comprador { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public string LugarEnvioCode { get; set; } = default!;
        public LugarEnvio LugarEnvio { get; set; } = default!;

        public string FormaPagoCode { get; set; } = default!;
        public FormaPago FormaPago { get; set; } = default!;

        public string TramiteCode { get; set; } = default!;
        public Tramite Tramite { get; set; } = default!;

        public string? Observaciones1 { get; set; }
        public string? Observaciones2 { get; set; }

        public string? NotaCompraCode { get; set; }
        public NotaCompra? NotaCompra { get; set; }

        /// <summary>Nueva: requerida por la matriz de aprobación. Default "000000" (igual que OrdenCompra).</summary>
        public string UnidadNegocioCode { get; set; } = default!;
        public UnidadNegocio UnidadNegocio { get; set; } = default!;

        public MotivoOrdenCompraAdministracion Motivo { get; set; } = MotivoOrdenCompraAdministracion.Otros;

        public EstadoOrdenCompraAdministracion Estado { get; set; } = EstadoOrdenCompraAdministracion.Pendiente;

        public string? AprobadoPor { get; set; }
        public DateTime? FechaAprobacion { get; set; }

        public decimal NetoCompra { get; set; }
        public decimal IgvCompra { get; set; }
        public decimal TotalCompra { get; set; }

        /// <summary>Digitado por el usuario, no se calcula.</summary>
        public decimal NoGravableCompra { get; set; }
        /// <summary>Calculado (ComprobantePago.AffectsIncomeTax + ControlVentas).</summary>
        public decimal RentaCompra { get; set; }
        /// <summary>Calculado (ComprobantePago.AffectsFonavi + ControlVentas).</summary>
        public decimal FonaviCompra { get; set; }
        /// <summary>Digitado por el usuario, no se calcula.</summary>
        public decimal ServicioCompra { get; set; }
        /// <summary>Digitado por el usuario, no se calcula.</summary>
        public decimal IgvExteriorCompra { get; set; }

        public ICollection<OrdenCompraAdministracionDetalle> Detalles { get; set; } = new List<OrdenCompraAdministracionDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
