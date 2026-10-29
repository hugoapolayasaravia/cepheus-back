// Cepheus.Application/Features/Logistica/Transacciones/Guias/Common/GuiaResponse.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.Common
{
    /// <summary>
    /// Guía de Remisión completa (cabecera + detalle). Incluye los datos que el
    /// legacy usaba para imprimir la guía (Logi_sp_Rpt_Guias): transportista,
    /// conductor, vehículo y destinatario.
    /// </summary>
    public class GuiaResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string? PlantaName { get; set; }

        /// <summary>Número completo 'SSS-NNNNNN'.</summary>
        public string Code { get; set; } = default!;
        public string Serie { get; set; } = default!;
        public string Numero { get; set; } = default!;

        public DateTime FechaEmision { get; set; }
        public string Hora { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public string? ProveedorDocumentNumber { get; set; }

        public string MotivoCode { get; set; } = default!;
        public string? MotivoName { get; set; }

        public string Direccion { get; set; } = default!;
        public string PuntoPartida { get; set; } = default!;

        public string TransportistaCode { get; set; } = default!;
        public string? TransportistaName { get; set; }
        public string? TransportistaDocumentNumber { get; set; }
        public string? TransportistaAddress { get; set; }
        public string? TransportistaCertificationNumber { get; set; }

        public string ConductorCode { get; set; } = default!;
        public string? ConductorName { get; set; }
        public string? ConductorDriverLicenseNumber { get; set; }

        public string VehiculoCode { get; set; } = default!;
        public string? VehiculoLicensePlate { get; set; }
        public string? VehiculoBrand { get; set; }

        public string Observaciones { get; set; } = default!;

        public string Estado { get; set; } = default!;

        public List<GuiaDetalleResponse> Detalles { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
