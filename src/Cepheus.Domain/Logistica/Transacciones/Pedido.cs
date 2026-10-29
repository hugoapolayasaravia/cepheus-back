// Cepheus.Domain/Logistica/Transacciones/Pedido.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Mantenimiento.Transacciones;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Pedido: cabecera de una solicitud de artículos/servicios generada
    /// por cualquier área de la empresa (no solo Logística). Transacción
    /// principal del módulo Logística.
    ///
    /// Legacy: dbo.MPedidoRes (SQL Server), PK compuesta
    /// (Codigo_Pla, Codigo_Ped) — mismo criterio de PK compuesta que
    /// OrdenTrabajo en Mantenimiento.
    ///
    /// Cambios frente al legacy (ver análisis previo aprobado):
    ///   - Cod_Planta: se CONSERVA tal cual (duplicado de Codigo_Pla en el
    ///     legacy) por pedido explícito del usuario — a diferencia de
    ///     OrdenTrabajo, aquí NO se elimina. Es un campo plano sin FK
    ///     propia, se llena con el mismo valor de PlantaCode al crear.
    ///   - Codigo_Est (char(2)) -> Estado (enum EstadoPedido)
    ///   - Usuario (varchar(20) libre) -> eliminado, lo cubre CreatedBy
    ///     (IAuditableEntity)
    ///   - Codigo_Tra (Trabajador) es obligatorio (confirmado por el
    ///     usuario, pese a que el legacy tenía un DEFAULT '' sobre una
    ///     columna NOT NULL)
    ///   - Codigo_Ped: correlativo por planta, formato '7' + 5 dígitos
    ///     (rango 700001-799999), generado por la aplicación
    ///   - Codigo_Otr -> OrdenTrabajoCode, ahora sí con FK real hacia
    ///     Mantenimiento.Transacciones.OrdenTrabajo (ya existe en el repo)
    ///
    /// Neto/Igv/Total se recalculan siempre desde las líneas de
    /// PedidoDetalle + Comunes.ControlVentas.IgvPercentage (fila única de
    /// configuración) — no se reciben del cliente.
    ///
    /// Flujo de negocio (contexto, no implementado en este alcance):
    ///   Pedido -> (Logística cotiza y) agrupa 1+ Pedidos en una Orden de
    ///   Compra (módulo futuro) -> Nota de Ingreso (módulo futuro) actualiza
    ///   stock y el estado tanto de la Orden de Compra como de los Pedidos
    ///   asociados. Por eso PedidoDetalle.OrdenCompraCode queda como campo
    ///   inerte reservado (igual criterio que los campos contables en
    ///   Articulo).
    /// </summary>
    public class Pedido : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        /// <summary>Correlativo por planta: '7' + 5 dígitos (Codigo_Ped, char(6)).</summary>
        public string Code { get; set; } = default!;

        /// <summary>
        /// Duplicado histórico de PlantaCode, conservado por pedido
        /// explícito del usuario (Cod_Planta en el legacy). Sin FK propia.
        /// </summary>
        public string CodPlanta { get; set; } = default!;

        public string TipoPedidoCode { get; set; } = default!;
        public TipoPedido TipoPedido { get; set; } = default!;

        public string TipoValeCode { get; set; } = default!;
        public TipoVale? TipoVale { get; set; }

        public string TramiteCode { get; set; } = default!;
        public Tramite Tramite { get; set; } = default!;

        public string SubCentroCostoCode { get; set; } = default!;
        public SubCentroCosto SubCentroCosto { get; set; } = default!;

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        /// <summary>Orden de Trabajo de Mantenimiento asociada (opcional).</summary>
        public string? OrdenTrabajoCode { get; set; }
        public OrdenTrabajo? OrdenTrabajo { get; set; }

        public string UnidadNegocioCode { get; set; } = default!;
        public UnidadNegocio UnidadNegocio { get; set; } = default!;

        public DateTime FechaEntrega { get; set; }

        public decimal NetoPedido { get; set; }
        public decimal IgvPedido { get; set; }
        public decimal TotalPedido { get; set; }

        public EstadoPedido EstadoPedido { get; set; } = EstadoPedido.Pendiente;

        public string Observaciones { get; set; } = string.Empty;

        public string? AprobadoPor { get; set; }
        public DateTime? FechaAprobacion { get; set; }

        public string? CompradoPor { get; set; }
        public DateTime? FechaCompra { get; set; }

        public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}