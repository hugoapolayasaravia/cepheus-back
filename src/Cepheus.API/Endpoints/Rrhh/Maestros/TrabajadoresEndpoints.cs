using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.CreateTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.GetTrabajadorByCode;
using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.GetTrabajadoresPaginated;
using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.UpdateTrabajador;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadoresEndpoints
    {
        public static void MapTrabajadoresEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajadores")
                .WithTags("Trabajadores (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTrabajadorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajadores/{result.Code}", result);
            })
            .WithName("CreateTrabajador")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTrabajadoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTrabajadoresPagedQueryString")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPost("/paged/body", async (
                GetTrabajadoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTrabajadoresPagedBody")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorByCode")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTrabajadorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajador")
            .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}