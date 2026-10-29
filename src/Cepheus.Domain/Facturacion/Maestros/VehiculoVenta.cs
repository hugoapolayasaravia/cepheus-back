using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Vehículo de un transportista. Entidad maestra del módulo de Facturación y Ventas.
    ///
    /// NO confundir con Logistica.Maestros.Vehiculo: mismo nombre, otra tabla y otros
    /// datos (schema "facturacion").
    ///
    /// Legacy: dbo.TVehiculos (SQL Server). PK compuesta (codigo_tra, Tipo_veh,
    /// codigo_veh): se mantiene como PK compuesta (TransportistaCode, VehicleType, Code),
    /// mismo criterio que OrdenTrabajo/ArticuloProveedor. El código es correlativo por
    /// transportista y tipo.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_tra    -> TransportistaCode (FK -> Facturacion.Transportista.Code; en el
    ///                    legacy la FK se creó WITH NOCHECK, puede haber huérfanos)
    ///   Tipo_veh      -> VehicleType (enum TipoVehiculo, parte de la PK)
    ///   codigo_veh    -> Code (char(4))
    ///   placa_veh     -> LicensePlate (char(8) -> 10, obligatoria)
    ///   marca_veh     -> Brand;  modelo_veh -> Model
    ///   codigo_cho    -> ChoferCode (opcional: chofer habitual. El legacy usaba un espacio
    ///                    como "sin chofer" y no tenía FK; ahora FK compuesta
    ///                    (TransportistaCode, ChoferCode) -> Chofer)
    ///   capacidad_veh -> Capacity (numeric(24,2) -> decimal(12,2))
    ///   suple_veh     -> Suple ("Suple")
    ///   largo_veh, ancho_veh, alto_veh -> LengthM, WidthM, HeightM
    ///   teles_veh     -> Telescopic ("Telescópica")
    ///   ss_veh, cs_veh           -> WithoutSuple, WithSuple ("Sin Suple" / "Con Suple")
    ///   cubss_veh, cubcs_veh     -> CubicWithoutSuple, CubicWithSuple ("Cubicaje sin/con Suple")
    ///   cimtc_veh     -> MtcInternalCode ("Código interno del MTC"; opcional)
    ///   configura_veh -> VehicularConfiguration (nvarchar(15) -> 30; opcional)
    ///   planilla_veh  -> PlanillaCode (char(10); significado a confirmar, se conserva)
    ///   observa_veh   -> Observations (varchar(200) -> 500)
    ///   Login_apr, Fecha_apr -> ApprovedBy, ApprovedAt (aprobación del vehículo; solo
    ///                    lectura por API hasta definir el flujo de aprobación)
    ///   codigo_usu, fecha_pro -> reemplazados por CreatedBy / CreatedAt (auditoría);
    ///                    el ETL debe cargarlos en esas columnas
    ///   codigo_est    -> reemplazado por IsActive (el legacy tenía un código de estado
    ///                    con default '05'; falta confirmar su significado)
    ///
    /// Unidades y fórmula de Suple / WithSuple / WithoutSuple / Cubic* / Telescopic
    /// pendientes de confirmar: Cubic* podría ser derivable de largo x ancho x altura
    /// (ver docs/migracion/Vehiculo_validaciones.sql). Por ahora se conservan todas.
    ///
    /// Regla nueva: UNIQUE(TransportistaCode, LicensePlate). El legacy no la tenía.
    /// </summary>
    public class VehiculoVenta : IAuditableEntity
    {
        public string TransportistaCode { get; set; } = default!;
        public TransportistaVenta TransportistaVenta { get; set; } = default!;

        public TipoVehiculo VehicleType { get; set; }

        /// <summary>Correlativo por transportista y tipo (codigo_veh, char(4) en el legacy).</summary>
        public string Code { get; set; } = default!;

        public string LicensePlate { get; set; } = default!;

        public string? Brand { get; set; }
        public string? Model { get; set; }

        public string? ChoferCode { get; set; }
        public ChoferVenta? ChoferVenta { get; set; }

        public decimal Capacity { get; set; }
        public decimal Suple { get; set; }
        public decimal LengthM { get; set; }
        public decimal WidthM { get; set; }
        public decimal HeightM { get; set; }
        public decimal Telescopic { get; set; }
        public decimal WithoutSuple { get; set; }
        public decimal WithSuple { get; set; }
        public decimal CubicWithoutSuple { get; set; }
        public decimal CubicWithSuple { get; set; }

        public string? MtcInternalCode { get; set; }
        public string? VehicularConfiguration { get; set; }
        public string? PlanillaCode { get; set; }

        public string? Observations { get; set; }

        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

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
