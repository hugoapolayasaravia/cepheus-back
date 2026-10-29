using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.CreateProveedorDireccion
{
    public record CreateProveedorDireccionCommand(
        string ProveedorCode,
        AddressType AddressType,
        string Address,
        string? UbigeoCode,
        string? Reference,
        bool IsPrimary
    ) : IRequest<ProveedorDireccionResponse>;
}
