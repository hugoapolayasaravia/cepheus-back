using Cepheus.Domain.Contabilidad.Enum;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common
{
    public class AsientoContableDetalleResponse
    {
        public int ItemNumber { get; set; }
        public string PlantaCode { get; set; } = default!;
        public bool EsDestino { get; set; }
        public string Descripcion { get; set; } = default!;
        public string CuentaContableCode { get; set; } = default!;
        public string? AnexoTipoCode { get; set; }
        public string? AnexoCode { get; set; }
        public string? NumeroDocumento { get; set; }
        public decimal CargoSol { get; set; }
        public decimal AbonoSol { get; set; }
        /// <summary>Derivado: CargoSol / TipoCambio del asiento (no se almacena).</summary>
        public decimal CargoDol { get; set; }
        /// <summary>Derivado: AbonoSol / TipoCambio del asiento (no se almacena).</summary>
        public decimal AbonoDol { get; set; }
        public TipoContabilidad TipoContabilidad { get; set; }
        public string? PlantaEjecutoraCode { get; set; }
        public string? ComprobantePagoCode { get; set; }
    }

    public class AsientoContableResponse
    {
        public string Codigo { get; set; } = default!;
        public string Anio { get; set; } = default!;
        public string Mes { get; set; } = default!;
        public string TipoAsientoCode { get; set; } = default!;
        public string? TipoAsientoName { get; set; }
        /// <summary>Histórico o Ajustado: viene de TipoAsiento.Valuation (era Tipo_tas en el legacy).</summary>
        public ValorizacionAsiento? Valuation { get; set; }
        public string Numero { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
        public DateTime FechaProceso { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal TotalCargosSol { get; set; }
        public decimal TotalAbonosSol { get; set; }
        public decimal TotalCargosDol { get; set; }
        public decimal TotalAbonosDol { get; set; }
        public EstadoAsiento Estado { get; set; }
        public OrigenAsiento Origen { get; set; }
        public List<AsientoContableDetalleResponse> Detalles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    public class AsientoContableListItemResponse
    {
        public string Codigo { get; set; } = default!;
        public string Anio { get; set; } = default!;
        public string Mes { get; set; } = default!;
        public string TipoAsientoCode { get; set; } = default!;
        public string Numero { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
        public DateTime FechaProceso { get; set; }
        public decimal TotalCargosSol { get; set; }
        public decimal TotalAbonosSol { get; set; }
        public EstadoAsiento Estado { get; set; }
        public OrigenAsiento Origen { get; set; }
        public string? CreatedBy { get; set; }
    }
}
