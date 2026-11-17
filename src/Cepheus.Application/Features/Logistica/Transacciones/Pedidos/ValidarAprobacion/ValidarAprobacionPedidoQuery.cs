using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.ValidarAprobacion
{
    /// <summary>
    /// Paso previo del cuadro «Aprobación por usuario»: comprueba login y clave y si ese usuario puede aprobar
    /// (o devolver a Pendiente) el pedido. No modifica nada; la acción se confirma con PATCH .../estado.
    /// </summary>
    public record ValidarAprobacionPedidoQuery(
        string PlantaCode,
        string Code,
        string? Login,
        string? Clave
    ) : IRequest<ValidarAprobacionPedidoResponse>;

    public record ValidarAprobacionPedidoResponse(
        /// <summary>"Aprobar" (Pendiente/AprobacionProvisional) o "Desaprobar" (Aprobado -> Pendiente).</summary>
        string Accion,
        string AprobadorLogin,
        string AprobadorNombre,
        string? TrabajadorCode,
        string? NivelCode,
        string? Rol,
        decimal Monto,
        string MonedaCode,
        string Mensaje
    );
}
