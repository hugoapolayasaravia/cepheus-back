using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.GetProveedorContactosByProveedor
{
    public record GetProveedorContactosByProveedorQuery(string ProveedorCode) : IRequest<List<ProveedorContactoResponse>>;
}
