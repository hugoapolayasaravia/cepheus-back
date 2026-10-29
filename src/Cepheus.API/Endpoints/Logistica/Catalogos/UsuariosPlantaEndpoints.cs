using Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.CreateUsuarioPlanta;
using Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.DeleteUsuarioPlanta;
using Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.GetUsuariosPlantaPaginated;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class UsuariosPlantaEndpoints
    {
        public static void MapUsuariosPlantaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/usuarios-planta")
                .WithTags("Usuarios por Planta (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateUsuarioPlantaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/catalogos/usuarios-planta/{result.UserId}/{result.PlantaCode}",
                    result);
            })
            .WithName("CreateUsuarioPlanta")
            .RequireAuthorization("USUARIOSPLANTA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetUsuariosPlantaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUsuariosPlantaPagedQueryString")
            .RequireAuthorization("USUARIOSPLANTA.VIEW");

            group.MapPost("/paged/body", async (
                GetUsuariosPlantaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUsuariosPlantaPagedBody")
            .RequireAuthorization("USUARIOSPLANTA.VIEW");

            group.MapDelete("/{userId:int}/{plantaCode}", async (
                int userId, string plantaCode, ISender sender) =>
            {
                await sender.Send(new DeleteUsuarioPlantaCommand(userId, plantaCode));
                return Results.NoContent();
            })
            .WithName("DeleteUsuarioPlanta")
            .RequireAuthorization("USUARIOSPLANTA.DELETE");
        }
    }
}
