using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.CreateArticuloPlantaUbicaciones;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.DeleteArticuloPlantaUbicaciones;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.GetArticuloPlantaUbicacionesByArticulo;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.GetArticuloPlantaUbicacionesByKey;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.UpdateArticuloPlantaUbicaciones;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ArticuloPlantaUbicacionesEndpoints
    {
        public static void MapArticuloPlantaUbicacionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/articulo-planta-ubicaciones")
                .WithTags("Artículo-Planta-Ubicación (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateArticuloPlantaUbicacionesCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/maestros/articulo-planta-ubicaciones/{result.ArticuloCode}/{result.PlantaCode}",
                    result);
            })
            .WithName("CreateArticuloPlantaUbicacion")
            .RequireAuthorization("ARTICULOS.UPDATE");

            group.MapGet("/articulo/{codigoArticulo}", async (string codigoArticulo, ISender sender) =>
            {
                var result = await sender.Send(new GetArticuloPlantaUbicacionesByArticuloQuery(codigoArticulo));
                return Results.Ok(result);
            })
            .WithName("GetArticuloPlantaUbicacionesByArticulo")
            .RequireAuthorization("ARTICULOS.VIEW");

            group.MapGet("/{codigoArticulo}/{codigoPlanta}", async (
                string codigoArticulo, string codigoPlanta, ISender sender) =>
            {
                var result = await sender.Send(new GetArticuloPlantaUbicacionesByKeyQuery(codigoArticulo, codigoPlanta));
                return Results.Ok(result);
            })
            .WithName("GetArticuloPlantaUbicacionesByKey")
            .RequireAuthorization("ARTICULOS.VIEW");

            group.MapPut("/{codigoArticulo}/{codigoPlanta}", async (
                string codigoArticulo, string codigoPlanta,
                UpdateArticuloPlantaUbicacionesCommand command, ISender sender) =>
            {
                if (codigoArticulo != command.ArticuloCode || codigoPlanta != command.PlantaCode)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateArticuloPlantaUbicacion")
            .RequireAuthorization("ARTICULOS.UPDATE");

            group.MapDelete("/{codigoArticulo}/{codigoPlanta}", async (
                string codigoArticulo, string codigoPlanta, ISender sender) =>
            {
                await sender.Send(new DeleteArticuloPlantaUbicacionesCommand(codigoArticulo, codigoPlanta));
                return Results.NoContent();
            })
            .WithName("DeleteArticuloPlantaUbicacion")
            .RequireAuthorization("ARTICULOS.UPDATE");
        }
    }
}