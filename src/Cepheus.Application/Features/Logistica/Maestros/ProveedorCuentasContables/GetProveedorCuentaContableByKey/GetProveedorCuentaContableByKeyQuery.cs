using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.GetProveedorCuentaContableByKey
{
    public record GetProveedorCuentaContableByKeyQuery(string ProveedorCode, string MonedaCode)
        : IRequest<ProveedorCuentaContableResponse>;
}
