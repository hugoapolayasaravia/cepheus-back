using Cepheus.Application.Features.Administracion.Submodulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Submodulos.GetSubmoduloById
{
    public record GetSubmoduloByIdQuery(int Id) : IRequest<SubmoduloResponse>;
}
