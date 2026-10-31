using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.ExportCuentasContables
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportCuentasContablesQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public int? Digits { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}
