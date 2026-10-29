using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.CreateTitulo
{
    public record CreateTituloCommand(
        string Name
    ) : IRequest<TituloResponse>;
}