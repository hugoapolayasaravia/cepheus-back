using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.CreateElementoConcreto;
using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.GetElementoConcretoByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.GetElementosConcretoPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.ToggleElementoConcretoStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.UpdateElementoConcreto;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class ElementosConcretoEndpoints
    {
        public static void MapElementosConcretoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/elementos-concreto")
                .WithTags("Elementos de Concreto (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateElementoConcretoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/elementos-concreto/{result.Code}", result);
            })
            .WithName("CreateElementoConcreto")
            .RequireAuthorization("ELEMENTOSCONCRETO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetElementosConcretoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetElementosConcretoPagedQueryString")
            .RequireAuthorization("ELEMENTOSCONCRETO.VIEW");

            group.MapPost("/paged/body", async (
                GetElementosConcretoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetElementosConcretoPagedBody")
            .RequireAuthorization("ELEMENTOSCONCRETO.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetElementoConcretoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetElementoConcretoByCode")
            .RequireAuthorization("ELEMENTOSCONCRETO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateElementoConcretoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateElementoConcreto")
            .RequireAuthorization("ELEMENTOSCONCRETO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleElementoConcretoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleElementoConcretoStatus")
            .RequireAuthorization("ELEMENTOSCONCRETO.UPDATE");
        }
    }
}
