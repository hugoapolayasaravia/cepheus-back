using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Hoja de Corte (cabecera): documento de producción de Facturación y Ventas que
    /// detalla las viguetas (cortes por paño, longitud y cables) y las bovedillas que
    /// se despachan a una obra. Aggregate root: HojaCorteDetalle solo existe a través
    /// de una HojaCorte.
    ///
    /// Legacy: dbo.RHojaCorte (SQL Server), PK compuesta (ano_hco, codigo_hco). Se
    /// mantiene la PK compuesta: el correlativo es por año (fn_NuevaHojaCorte).
    /// Pantalla PB: w_ges_reg_hoja_corte. Búsqueda: ges_sp_bus_hoja_corte.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   ano_hco, codigo_hco  -> Year, Code (PK compuesta)
    ///   codigo_thc           -> TipoHojaCorteCode (FK -> Facturacion.TipoHojaCorte)
    ///   codigo_cli           -> ClienteCode (FK -> Cliente, OPCIONAL: el DDL lo declara
    ///                           NOT NULL pero su comentario y el PB permiten una HC sin
    ///                           cliente; el PB graba NULL cuando viene vacío)
    ///   Cliente_cot          -> ClientName (snapshot de la razón social)
    ///   codigo_obr           -> ObraCode (FK compuesta opcional ClienteCode+ObraCode -> Obra)
    ///   Obra_cot             -> WorkName (snapshot)
    ///   Direccion_obr        -> WorkAddress (snapshot)
    ///   UbiDireccion_obr     -> WorkAddressUbigeoCode (FK -> Comunes.Ubigeo)
    ///   tecnico_hco          -> TecnicoCode (FK -> Facturacion.Tecnico)
    ///   Codigo_ven           -> VendedorCode (FK -> Facturacion.Vendedor; "Ejecutivo")
    ///   Sector_hco           -> Sector (ingreso manual)
    ///   piso_hco             -> Floor ("Piso o Nivel", ingreso manual)
    ///   fecha_hco            -> Date (fecha de la HC, solo fecha)
    ///   fecdes_hco           -> ViguetaDispatchDate (F. Desp. Viguetas, solo fecha)
    ///   fecdesbov_hco        -> BovedillaDispatchDate (F. Desp. Bovedillas, solo fecha)
    ///   fecclides_hco        -> ClientViguetaDispatchDate (fecha posible Desp. Viguetas
    ///                           Cli, fecha y hora; obligatoria al grabar según el PB)
    ///   fecclidesbov_hco     -> ClientBovedillaDispatchDate (ídem Bovedillas)
    ///   FechaAlm_hco         -> StorageDate (Fecha Almacenaje, solo fecha)
    ///   codigo_ord           -> OrdenTrabajoCode (Orden de Trabajo de Ventas)
    ///                           DE FK REAL: la entidad de OT de Ventas se está creando;
    ///                           se guarda el código sin navegación EF hasta que exista
    ///                           (mismo criterio que Obra.CobradorCode antes de Cobrador).
    ///   codigo_est           -> Status (enum EstadoHojaCorte)
    ///   observacion_hco      -> Observations
    ///   Login_usr            -> CreatedBy (auditoría; el DDL indica reemplazarlo)
    ///   fecproceso_hco       -> CreatedAt (auditoría; el DDL indica reemplazarlo)
    ///   Programado_hco       -> IsScheduled (char(1) -> bool; Fase 2: programación de HC)
    ///   hco_ano, hco_codigo  -> ParentYear, ParentCode (auto-FK compuesta opcional:
    ///                           HC padre en Adicional/Reemplazo, y HC de origen al copiar)
    ///   Turno_hco            -> Turn (enum TurnoHojaCorte)
    ///   Ind_Modificable      -> IsModifiable (char(1) S/N -> bool). En false la HC queda
    ///                           bloqueada: nadie puede modificarla (queda para aprobación).
    ///   Motivo_hco           -> Motivo (enum MotivoHojaCorte, solo tipo Reemplazo)
    ///   TipoPro_hco          -> ProductType (enum TipoProductoHojaCorte)
    ///   Estado_doc           -> OMITIDO por ahora: depende de la parte de facturación
    ///                           (estado del documento de venta de la OT), aún no construida.
    ///   numero_npd, codigo_des, codigo_pre, codigo_jtr, numero_gve (usados por el PB
    ///   pero fuera del DDL entregado) -> omitidos: el usuario indicó que no se usan.
    /// </summary>
    public class HojaCorte : IAuditableEntity
    {
        /// <summary>Año de la hoja de corte (char(4) en el legacy).</summary>
        public string Year { get; set; } = default!;

        /// <summary>Correlativo por año (char(8) en el legacy).</summary>
        public string Code { get; set; } = default!;

        public string TipoHojaCorteCode { get; set; } = default!;
        public TipoHojaCorte TipoHojaCorte { get; set; } = default!;

        public string? ClienteCode { get; set; }
        public Cliente? Cliente { get; set; }

        public string ClientName { get; set; } = default!;

        public string? ObraCode { get; set; }
        public Obra? Obra { get; set; }

        public string WorkName { get; set; } = default!;
        public string WorkAddress { get; set; } = default!;

        public string WorkAddressUbigeoCode { get; set; } = default!;
        public Ubigeo WorkAddressUbigeo { get; set; } = default!;

        public string TecnicoCode { get; set; } = default!;
        public Tecnico Tecnico { get; set; } = default!;

        public string VendedorCode { get; set; } = default!;
        public Vendedor Vendedor { get; set; } = default!;

        public string Sector { get; set; } = default!;
        public string Floor { get; set; } = default!;

        public DateTime Date { get; set; }

        public DateTime? ViguetaDispatchDate { get; set; }
        public DateTime? BovedillaDispatchDate { get; set; }

        public DateTime? ClientViguetaDispatchDate { get; set; }
        public DateTime? ClientBovedillaDispatchDate { get; set; }

        public DateTime? StorageDate { get; set; }

        /// <summary>Orden de Trabajo de Ventas asociada (opcional).</summary>
        public string? OrdenTrabajoCode { get; set; }
        public OrdenTrabajoVenta? OrdenTrabajo { get; set; }

        public EstadoHojaCorte Status { get; set; } = EstadoHojaCorte.Pendiente;

        public string? Observations { get; set; }

        public bool IsScheduled { get; set; }

        public string? ParentYear { get; set; }
        public string? ParentCode { get; set; }
        public HojaCorte? Parent { get; set; }

        public TurnoHojaCorte Turn { get; set; }

        public bool IsModifiable { get; set; } = true;

        public MotivoHojaCorte? Motivo { get; set; }

        public TipoProductoHojaCorte? ProductType { get; set; }

        // Hijos del aggregate
        public ICollection<HojaCorteDetalle> Detalles { get; set; } = new List<HojaCorteDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
