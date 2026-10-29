using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Rango de monto que un Nivel puede aprobar, para una combinación de
    /// Tipo de Transacción + Unidad de Negocio + Moneda. El corazón de la
    /// matriz de aprobación. Maestro del submódulo de Gestión de Aprobación
    /// de Transacciones, dentro de Logística.
    ///
    /// Legacy: dbo.Parametria_Niveles_de_Aprobacion (SQL Server), PK
    /// compuesta (Codigo_Niv, Codigo_Tras, Codigo_Une, Codigo_Mon).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Niv         -> NivelCode (FK -> Nivel)
    ///   Codigo_Tras        -> TipoTransaccionCode (FK -> TipoTransaccion)
    ///   Codigo_Une         -> UnidadNegocioCode (FK -> UnidadNegocio, ya
    ///                         existe en Logistica.Catalogos)
    ///   Codigo_Mon         -> MonedaCode (FK -> Moneda, ya existe en Comunes)
    ///   Importe_Min        -> ImporteMinimo
    ///   Importe_Max        -> ImporteMaximo
    ///   Importe_Acum_Dia   -> ImporteAcumuladoDiario
    ///   Importe_Acum_Men   -> ImporteAcumuladoMensual
    ///   Porcentaje_Tot     -> PorcentajeTotal (nullable — solo tiene
    ///                         sentido para la transacción "LC" - Límite de
    ///                         Crédito, confirmado con el usuario; para el
    ///                         resto de transacciones queda null)
    ///
    /// Resolución con herencia: si una UnidadNegocio no tiene RangoAprobacion
    /// propio para (TipoTransaccion, Moneda), se debe subir por
    /// UnidadNegocio.ParentCode hasta encontrar uno (ver
    /// ResolverRangoAprobacionQuery) — confirmado con el usuario a partir del
    /// árbol de unidades de negocio.
    /// </summary>
    public class RangoAprobacion : IAuditableEntity
    {
        public string NivelCode { get; set; } = default!;
        public Nivel Nivel { get; set; } = default!;

        public string TipoTransaccionCode { get; set; } = default!;
        public TipoTransaccion TipoTransaccion { get; set; } = default!;

        public string UnidadNegocioCode { get; set; } = default!;
        public UnidadNegocio UnidadNegocio { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public decimal ImporteMinimo { get; set; }
        public decimal ImporteMaximo { get; set; }
        public decimal ImporteAcumuladoDiario { get; set; }
        public decimal ImporteAcumuladoMensual { get; set; }
        public decimal? PorcentajeTotal { get; set; }

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
