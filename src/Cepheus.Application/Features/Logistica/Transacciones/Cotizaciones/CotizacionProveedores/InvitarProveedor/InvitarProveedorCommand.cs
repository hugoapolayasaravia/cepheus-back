// Cepheus.Application/Features/Logistica/Transacciones/CotizacionProveedores/InvitarProveedor/InvitarProveedorCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.InvitarProveedor
{
    public record InvitarProveedorCommand(
        string PlantaCode,
        string CotizacionCode,
        string ProveedorCode,
        string MonedaCode,
        string? Observaciones
    ) : IRequest<CotizacionResponse>;
}