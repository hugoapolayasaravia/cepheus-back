using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.GetProveedorDireccionById
{
    public record GetProveedorDireccionByIdQuery(int Id) : IRequest<ProveedorDireccionResponse>;
}
