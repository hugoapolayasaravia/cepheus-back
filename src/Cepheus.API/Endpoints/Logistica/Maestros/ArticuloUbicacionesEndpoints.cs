using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.CreateArticuloUbicaciones;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicacionById;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicaciones;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicacionesByArticulo;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicacionesByPlanta;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.UpdateArticuloUbicaciones;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ArticuloUbicacionesEndpoints
    {
        public static void MapArticuloUbicacionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/articulo-ubicaciones")
                .WithTags("Artículo Ubicaciones (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateArticuloUbicacionesCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/articulo-ubicaciones/{result.Code}", result);
            })
            .WithName("CreateArticuloUbicacion")
            .RequireAuthorization("ARTICULOS.UPDATE");

            group.MapGet("/", async (ISender sender) =>
            {
                var result = await sender.Send(new GetArticuloUbicacionesQuery());
                return Results.Ok(result);
            })
            .WithName("GetArticuloUbicaciones")
            .RequireAuthorization("ARTICULOS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetArticuloUbicacionByIdQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetArticuloUbicacionById")
            .RequireAuthorization("ARTICULOS.VIEW");

            group.MapGet("/planta/{codigoPlanta}", async (string codigoPlanta, ISender sender) =>
            {
                var result = await sender.Send(new GetArticuloUbicacionesByPlantaQuery(codigoPlanta));
                return Results.Ok(result);
            })
            .WithName("GetArticuloUbicacionesByPlanta")
            .RequireAuthorization("ARTICULOS.VIEW");

            group.MapGet("/articulo/{codigoArticulo}", async (string codigoArticulo, ISender sender) =>
            {
                var result = await sender.Send(new GetArticuloUbicacionesByArticuloQuery(codigoArticulo));
                return Results.Ok(result);
            })
            .WithName("GetArticuloUbicacionesByArticulo")
            .RequireAuthorization("ARTICULOS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateArticuloUbicacionesCommand command, ISender sender) =>
            {
                if (codigo != command.Code)
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateArticuloUbicacion")
            .RequireAuthorization("ARTICULOS.UPDATE");
        }
    }
}