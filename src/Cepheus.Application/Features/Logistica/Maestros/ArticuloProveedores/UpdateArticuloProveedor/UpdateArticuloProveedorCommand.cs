using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.UpdateArticuloProveedor
{
    // Solo se edita lo que no es parte de la clave compuesta.
    public record UpdateArticuloProveedorCommand(
        string PlantaCode,
        string ArticuloCode,
        string ProveedorCode,
        bool IsAgreement,
        decimal? AgreementPrice
    ) : IRequest<ArticuloProveedorResponse>;
}
