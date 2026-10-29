using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.GetNotasCotizacionPlantillaPaginated
{
    public class GetNotasCotizacionPlantillaPaginatedQuery : PagedRequest, IRequest<PagedResult<NotaCotizacionPlantillaResponse>>
    {
        public string? NegocioCode { get; set; }
        public OpcionNotaCotizacion? Option { get; set; }
        public bool? IsActive { get; set; }
        public string? Search { get; set; }
    }
}
