using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.GetProveedorCuentaById
{
    public record GetProveedorCuentaByIdQuery(int Id) : IRequest<ProveedorCuentaResponse>;
}

