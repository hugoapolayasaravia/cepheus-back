using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.CreateAlturaLosa;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.GetAlturaLosaByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.GetAlturasLosaPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.ToggleAlturaLosaStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.UpdateAlturaLosa;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class AlturasLosaEndpoints
    {
        public static void MapAlturasLosaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/alturas-losa")
                .WithTags("Alturas de Losa (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateAlturaLosaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/alturas-losa/{result.Code}", result);
            })
            .WithName("CreateAlturaLosa")
            .RequireAuthorization("ALTURASLOSA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetAlturasLosaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAlturasLosaPagedQueryString")
            .RequireAuthorization("ALTURASLOSA.VIEW");

            group.MapPost("/paged/body", async (
                GetAlturasLosaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAlturasLosaPagedBody")
            .RequireAuthorization("ALTURASLOSA.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetAlturaLosaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetAlturaLosaByCode")
            .RequireAuthorization("ALTURASLOSA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateAlturaLosaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateAlturaLosa")
            .RequireAuthorization("ALTURASLOSA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleAlturaLosaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleAlturaLosaStatus")
            .RequireAuthorization("ALTURASLOSA.UPDATE");
        }
    }
}
