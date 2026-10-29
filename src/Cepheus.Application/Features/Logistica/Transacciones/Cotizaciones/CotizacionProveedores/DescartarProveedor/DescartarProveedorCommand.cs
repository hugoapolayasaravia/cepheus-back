// Cepheus.Application/Features/Logistica/Transacciones/CotizacionProveedores/DescartarProveedor/DescartarProveedorCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.DescartarProveedor
{
    public record DescartarProveedorCommand(string PlantaCode, string CotizacionCode, string ProveedorCode) : IRequest<CotizacionResponse>;
}