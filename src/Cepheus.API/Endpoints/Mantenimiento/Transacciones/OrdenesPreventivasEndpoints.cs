using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesPreventivas.GenerateOrdenesPreventivas;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Transacciones
{
    public static class OrdenesPreventivasEndpoints
    {
        public static void MapOrdenesPreventivasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/transacciones/ordenes-preventivas")
                .WithTags("Órdenes de Trabajo preventivas (Mantenimiento)")
                .RequireAuthorization();

            // Genera las OT preventivas que tocan en el rango (y su vale de salida con los materiales del plan).
            group.MapPost("/generar", async (GenerateOrdenesPreventivasCommand command, ISender sender) =>
                Results.Ok(await sender.Send(command)))
            .WithName("GenerateOrdenesPreventivas")
            .RequireAuthorization("ORDENESTRABAJO.CREATE");
        }
    }
}
