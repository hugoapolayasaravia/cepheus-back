using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.GetProveedorCondicionById
{
    public record GetProveedorCondicionByIdQuery(int Id) : IRequest<ProveedorCondicionResponse>;
}
