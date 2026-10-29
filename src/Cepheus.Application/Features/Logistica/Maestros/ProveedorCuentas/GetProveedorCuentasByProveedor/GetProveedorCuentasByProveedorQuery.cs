using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.GetProveedorCuentasByProveedor
{
    public record GetProveedorCuentasByProveedorQuery(string ProveedorCode) : IRequest<List<ProveedorCuentaResponse>>;
}
