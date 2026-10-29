using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.CreateCategoriaTrabajador;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.GetCategoriaTrabajadorByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.GetCategoriasTrabajadorPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.ToggleCategoriaTrabajadorStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.UpdateCategoriaTrabajador;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class CategoriasTrabajadorEndpoints
    {
        public static void MapCategoriasTrabajadorEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/categorias-trabajador")
                .WithTags("CategoriasTrabajador (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCategoriaTrabajadorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/categorias-trabajador/{result.Code}", result);
            })
            .WithName("CreateCategoriaTrabajador")
            .RequireAuthorization("CATEGORIASTRABAJADOR.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCategoriasTrabajadorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCategoriasTrabajadorPagedQueryString")
            .RequireAuthorization("CATEGORIASTRABAJADOR.VIEW");

            group.MapPost("/paged/body", async (
                GetCategoriasTrabajadorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCategoriasTrabajadorPagedBody")
            .RequireAuthorization("CATEGORIASTRABAJADOR.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCategoriaTrabajadorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetCategoriaTrabajadorByCode")
            .RequireAuthorization("CATEGORIASTRABAJADOR.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateCategoriaTrabajadorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCategoriaTrabajador")
            .RequireAuthorization("CATEGORIASTRABAJADOR.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleCategoriaTrabajadorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleCategoriaTrabajadorStatus")
            .RequireAuthorization("CATEGORIASTRABAJADOR.UPDATE");
        }
    }
}