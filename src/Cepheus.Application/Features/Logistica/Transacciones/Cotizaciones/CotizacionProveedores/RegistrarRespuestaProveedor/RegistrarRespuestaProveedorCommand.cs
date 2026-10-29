// Cepheus.Application/Features/Logistica/Transacciones/CotizacionProveedores/RegistrarRespuestaProveedor/RegistrarRespuestaProveedorCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.RegistrarRespuestaProveedor
{
    /// <summary>
    /// Registra (o reemplaza por completo) la oferta de precios de un
    /// proveedor invitado. Pasa el proveedor a Respondido y setea
    /// FechaRespuesta. Recalcula Neto/Igv/Total.
    /// </summary>
    public record RegistrarRespuestaProveedorCommand(
        string PlantaCode,
        string CotizacionCode,
        string ProveedorCode,
        List<RegistrarRespuestaLineaInput> Lineas
    ) : IRequest<CotizacionResponse>;

    public record RegistrarRespuestaLineaInput(
        string ArticuloCode,
        decimal CantidadArticulo,
        decimal PrecioArticulo,
        decimal DescuentoArticulo
    );
}