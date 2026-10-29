using Cepheus.Application.Features.Administracion.Modulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Modulos.GetModuloById
{
    public record GetModuloByIdQuery(int Id) : IRequest<ModuloResponse>;
}
