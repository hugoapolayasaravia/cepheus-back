using Cepheus.Domain.Comun;
using Cepheus.Domain.Contabilidad.Catalogos;
using Cepheus.Domain.Contabilidad.Enum;

namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Asiento contable (cabecera). Legacy: dbo.MAsientoResumen (SQL Server).
    /// PK: (Anio, Mes, TipoAsientoCode, Numero). El código completo (13 caracteres) es la concatenación
    /// de los cuatro: año(4) + mes(2) + tipo(2) + número(5), igual que la columna calculada "asiento" del legacy.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   AnoProceso_res     -> Anio
    ///   MesProceso_res     -> Mes
    ///   Codigo_tas         -> TipoAsientoCode (FK Contabilidad.TipoAsiento)
    ///   Numero_res         -> Numero (correlativo por año, mes y tipo)
    ///   Descripcion_res    -> Descripcion (500)
    ///   Fechaproceso_res   -> FechaProceso (fecha contable del asiento)
    ///   TipoCambio_res     -> TipoCambio
    ///   TotCargosSol_res   -> TotalCargosSol
    ///   TotAbonosSol_res   -> TotalAbonosSol
    ///   Estado_res         -> Estado (enum EstadoAsiento)
    ///   origen_flg         -> Origen (enum OrigenAsiento)
    ///   Codigo_Usu / Fechasistema_res -> CreatedBy / CreatedAt (auditoría)
    ///   asiento (calculada) -> Codigo (propiedad calculada, no mapeada)
    ///
    /// Cambios frente al legacy:
    ///   - Los importes en dólares no se guardan: son derivados (importe en soles / TipoCambio) y se calculan
    ///     al consultar.
    ///   - Tipo_tas (histórico/ajustado) no se repite: se obtiene de TipoAsiento.Valuation.
    ///   - Tipo_con pasa a la línea (TipoContabilidad), que es donde tiene sentido.
    /// </summary>
    public class AsientoContable : IAuditableEntity
    {
        public string Anio { get; set; } = default!;
        public string Mes { get; set; } = default!;

        public string TipoAsientoCode { get; set; } = default!;
        public TipoAsiento TipoAsiento { get; set; } = default!;

        public string Numero { get; set; } = default!;

        /// <summary>Código completo del asiento: Anio + Mes + TipoAsientoCode + Numero (13 caracteres).</summary>
        public string Codigo => Anio + Mes + TipoAsientoCode + Numero;

        public string Descripcion { get; set; } = default!;

        public DateTime FechaProceso { get; set; }

        public decimal TipoCambio { get; set; }

        public decimal TotalCargosSol { get; set; }
        public decimal TotalAbonosSol { get; set; }

        public EstadoAsiento Estado { get; set; } = EstadoAsiento.Pendiente;

        public OrigenAsiento Origen { get; set; } = OrigenAsiento.Contabilidad;

        public ICollection<AsientoContableDetalle> Detalles { get; set; } = new List<AsientoContableDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
