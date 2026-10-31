using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.ExportCuentasBancarias
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportCuentasBancariasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? BankCode { get; set; }
        public string? CurrencyCode { get; set; }
    }
}
