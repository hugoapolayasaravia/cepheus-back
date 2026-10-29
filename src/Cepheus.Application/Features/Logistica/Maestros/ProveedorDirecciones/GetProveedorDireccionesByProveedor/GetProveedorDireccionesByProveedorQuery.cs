using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.GetProveedorDireccionesByProveedor
{
    public record GetProveedorDireccionesByProveedorQuery(string ProveedorCode) : IRequest<List<ProveedorDireccionResponse>>;
}
