using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.UpdateTitulo
{
    public record UpdateTituloCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TituloResponse>;
}