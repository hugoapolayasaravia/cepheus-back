using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.UpdateProveedorDireccion
{
    // ProveedorCode NO se edita: mover una dirección a otro proveedor es una
    // operación estructural distinta (mismo criterio que SubFamilia.FamiliaCode).
    public record UpdateProveedorDireccionCommand(
        int Id,
        AddressType AddressType,
        string Address,
        string? UbigeoCode,
        string? Reference,
        bool IsPrimary
    ) : IRequest<ProveedorDireccionResponse>;
}
