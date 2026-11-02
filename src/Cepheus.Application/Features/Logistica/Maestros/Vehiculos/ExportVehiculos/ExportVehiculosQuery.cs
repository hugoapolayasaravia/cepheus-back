using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.ExportVehiculos
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportVehiculosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? TransportistaCode { get; set; }
    }
}
