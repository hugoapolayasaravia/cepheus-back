using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.GetProveedorCondicionesByProveedor
{
    public record GetProveedorCondicionesByProveedorQuery(string ProveedorCode) : IRequest<List<ProveedorCondicionResponse>>;
}
