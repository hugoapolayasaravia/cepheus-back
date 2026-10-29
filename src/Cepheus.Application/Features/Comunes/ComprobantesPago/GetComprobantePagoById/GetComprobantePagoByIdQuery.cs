using Cepheus.Application.Features.Comunes.ComprobantesPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.GetComprobantePagoById
{
    public record GetComprobantePagoByIdQuery(string Code) : IRequest<ComprobantePagoResponse>;
}
