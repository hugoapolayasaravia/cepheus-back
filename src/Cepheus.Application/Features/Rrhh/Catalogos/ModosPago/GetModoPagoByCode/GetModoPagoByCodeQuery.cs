using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.GetModoPagoByCode
{
    public record GetModoPagoByCodeQuery(string Code) : IRequest<ModoPagoResponse>;
}