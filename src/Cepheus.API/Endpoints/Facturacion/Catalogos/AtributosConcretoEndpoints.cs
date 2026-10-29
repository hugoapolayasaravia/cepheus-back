using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.CreateAtributoConcreto;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.GetAtributoConcretoById;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.GetAtributosConcretoPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.ToggleAtributoConcretoStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.UpdateAtributoConcreto;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class AtributosConcretoEndpoints
    {
        public static void MapAtributosConcretoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/atributos-concreto")
                .WithTags("Atributos de concreto")
                .RequireAuthorization();

            group.MapPost("/", async (CreateAtributoConcretoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/atributos-concreto/{result.Code}", result);
            })
            .WithName("CreateAtributoConcreto")
            .RequireAuthorization("ATRIBUTOSCONCRETO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetAtributosConcretoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAtributosConcretoPagedQueryString")
            .RequireAuthorization("ATRIBUTOSCONCRETO.VIEW");

            group.MapPost("/paged/body", async (
                GetAtributosConcretoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAtributosConcretoPagedBody")
            .RequireAuthorization("ATRIBUTOSCONCRETO.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetAtributoConcretoByIdQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetAtributoConcretoById")
            .RequireAuthorization("ATRIBUTOSCONCRETO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateAtributoConcretoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateAtributoConcreto")
            .RequireAuthorization("ATRIBUTOSCONCRETO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleAtributoConcretoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleAtributoConcretoStatus")
            .RequireAuthorization("ATRIBUTOSCONCRETO.UPDATE");
        }
    }
}
