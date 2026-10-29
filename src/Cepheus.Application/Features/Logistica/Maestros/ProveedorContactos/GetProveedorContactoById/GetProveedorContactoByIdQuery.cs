using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.GetProveedorContactoById
{
    public record GetProveedorContactoByIdQuery(int Id) : IRequest<ProveedorContactoResponse>;
}
