using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.ExportVehiculos
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportVehiculosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? TransportistaCode { get; set; }
        public string? VehicleType { get; set; }
    }
}
