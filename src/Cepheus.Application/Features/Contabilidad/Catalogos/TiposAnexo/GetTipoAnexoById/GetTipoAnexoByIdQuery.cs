using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.GetTipoAnexoById
{
    public record GetTipoAnexoByIdQuery(string Code) : IRequest<TipoAnexoResponse>;
}
