using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.CreateArticuloProveedor
{
    public record CreateArticuloProveedorCommand(
        string PlantaCode,
        string ArticuloCode,
        string ProveedorCode,
        bool IsAgreement,
        decimal? AgreementPrice
    ) : IRequest<ArticuloProveedorResponse>;
}
