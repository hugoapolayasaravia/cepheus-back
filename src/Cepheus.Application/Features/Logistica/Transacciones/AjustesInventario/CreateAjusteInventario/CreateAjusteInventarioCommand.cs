using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.CreateAjusteInventario;

/// <summary>
/// Registro de un Ajuste de Inventario (queda Pendiente; hay que procesarlo para que afecte el stock).
/// Las líneas son opcionales al crear (el PB graba la cabecera y luego agrega líneas).
/// </summary>
public sealed class CreateAjusteInventarioCommand : IRequest<AjusteInventarioResponse>
{
    public string PlantaCode { get; set; } = default!;
    public DateTime FechaEntrega { get; set; }
    public string? Observacion { get; set; }
    public List<AjusteInventarioDetalleRequest> Detalles { get; set; } = new();
}
