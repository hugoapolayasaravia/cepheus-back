// Cepheus.Application/Features/Logistica/Transacciones/CotizacionProveedores/SeleccionarProveedor/SeleccionarProveedorCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.SeleccionarProveedor
{
    public record SeleccionarProveedorCommand(string PlantaCode, string CotizacionCode, string ProveedorCode) : IRequest<CotizacionResponse>;
}