using Cepheus.Application.Features.Comunes.ControlesVentas.CreateControlVentas;
using Cepheus.Application.Features.Comunes.ControlesVentas.GetControlVentas;
using Cepheus.Application.Features.Comunes.ControlesVentas.UpdateControlVentas;
using MediatR;

namespace Cepheus.API.Endpoints.Comunes
{
    public static class ControlesVentasEndpoints
    {
        public static void MapControlesVentasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/comunes/controles-ventas")
                .WithTags("ControlesVentas")
                .RequireAuthorization();

            // GET
            group.MapGet("/", async (ISender sender) =>
            {
                var result = await sender.Send(new GetControlVentasQuery());

                return Results.Ok(result);
            })
            .WithName("GetControlVentas")
            .RequireAuthorization("CONTROLESVENTAS.VIEW");

            // POST
            group.MapPost("/", async (
                CreateControlVentasCommand command,
                ISender sender) =>
            {
                var result = await sender.Send(command);

                return Results.Created(
                    $"/api/comunes/controles-ventas/{result.Id}",
                    result);
            })
            .WithName("CreateControlVentas")
            .RequireAuthorization("CONTROLESVENTAS.CREATE");

            // PUT
            group.MapPut("/{id:int}", async (
                int id,
                UpdateControlVentasCommand command,
                ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest(
                        "El Id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);

                return Results.Ok(result);
            })
            .WithName("UpdateControlVentas")
            .RequireAuthorization("CONTROLESVENTAS.UPDATE");
        }
    }
}