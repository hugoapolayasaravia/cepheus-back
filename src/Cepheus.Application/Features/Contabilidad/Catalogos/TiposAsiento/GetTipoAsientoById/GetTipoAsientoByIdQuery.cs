using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.GetTipoAsientoById
{
    public record GetTipoAsientoByIdQuery(string Code) : IRequest<TipoAsientoResponse>;
}
