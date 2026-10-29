using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.CreateOrdenServicioDetalle;

/// <summary>Agrega una línea a una Orden de Servicio Pendiente y recalcula los importes de la cabecera.</summary>
public sealed class CreateOrdenServicioDetalleCommand : IRequest<OrdenServicioResponse>
{
    public string PlantaCode { get; set; } = default!;
    public string OrdenServicioCode { get; set; } = default!;
    public OrdenServicioDetalleRequest Detalle { get; set; } = default!;
}
