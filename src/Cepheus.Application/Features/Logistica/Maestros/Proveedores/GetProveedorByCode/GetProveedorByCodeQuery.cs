using Cepheus.Application.Features.Logistica.Maestros.Proveedores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.GetProveedorByCode
{
    public record GetProveedorByCodeQuery(string Code) : IRequest<ProveedorResponse>;
}
