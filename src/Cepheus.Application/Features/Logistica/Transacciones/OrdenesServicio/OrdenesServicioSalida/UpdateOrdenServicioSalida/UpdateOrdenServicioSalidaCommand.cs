using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.UpdateOrdenServicioSalida;

/// <summary>
/// Cambia el trabajador responsable del vale de salida (en el legacy es lo único que se edita en la pestaña
/// "Orden de Servicio Salida"). Las líneas, fechas e importes del vale los gobierna la Orden de Servicio.
/// </summary>
public sealed class UpdateOrdenServicioSalidaCommand : IRequest<OrdenServicioSalidaResponse>
{
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string TrabajadorCode { get; set; } = default!;
}
