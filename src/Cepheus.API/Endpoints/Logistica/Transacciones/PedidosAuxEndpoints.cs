using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetMisPlantas;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetUsuariosPedido;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones
{
    public static class PedidosAuxEndpoints
    {
        /// <summary>Llamar desde Program.cs junto a MapPedidosEndpoints().</summary>
        public static void MapPedidosAuxEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/transacciones/pedidos-aux")
                .WithTags("Pedidos (Logística) - auxiliares")
                .RequireAuthorization();

            group.MapGet("/mis-plantas", async (ISender sender) =>
                Results.Ok(await sender.Send(new GetMisPlantasQuery())))
                .WithName("GetMisPlantasPedido")
                .RequireAuthorization("PEDIDOS.VIEW");

            group.MapGet("/usuarios", async (string? search, ISender sender) =>
                Results.Ok(await sender.Send(new GetUsuariosPedidoQuery(search))))
                .WithName("GetUsuariosPedido")
                .RequireAuthorization("PEDIDOS.VIEW");
        }
    }
}
