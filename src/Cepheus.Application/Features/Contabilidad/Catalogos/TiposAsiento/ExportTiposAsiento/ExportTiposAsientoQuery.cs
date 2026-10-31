using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.ExportTiposAsiento
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposAsientoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public TipoCambioAplicable? ExchangeRateType { get; set; }
        public ValorizacionAsiento? Valuation { get; set; }
    }
}
