using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.UpdateOrdenServicioDetalle;

/// <summary>
/// Modifica una línea (se identifica por ItemNumber). El artículo no se puede cambiar (en el legacy el código
/// de artículo queda bloqueado al modificar): para otro artículo se elimina la línea y se agrega una nueva.
/// El ArticuloCode del cuerpo, si se envía, debe coincidir con el de la línea.
/// </summary>
public sealed class UpdateOrdenServicioDetalleCommand : IRequest<OrdenServicioResponse>
{
    public string PlantaCode { get; set; } = default!;
    public string OrdenServicioCode { get; set; } = default!;
    public int ItemNumber { get; set; }
    public OrdenServicioDetalleRequest Detalle { get; set; } = default!;
}
