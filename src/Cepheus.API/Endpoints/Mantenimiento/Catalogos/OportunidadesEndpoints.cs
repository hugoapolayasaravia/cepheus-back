using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.CreateOportunidad;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.GetOportunidadByCode;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.GetOportunidadesPaginated;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.ToggleOportunidadStatus;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.UpdateOportunidad;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Catalogos
{
    public static class OportunidadesEndpoints
    {
        public static void MapOportunidadesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/catalogos/oportunidades")
                .WithTags("Oportunidades (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateOportunidadCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/catalogos/oportunidades/{result.Code}", result);
            })
            .WithName("CreateOportunidad")
            .RequireAuthorization("OPORTUNIDADES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetOportunidadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetOportunidadesPagedQueryString")
            .RequireAuthorization("OPORTUNIDADES.VIEW");

            group.MapPost("/paged/body", async (
                GetOportunidadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetOportunidadesPagedBody")
            .RequireAuthorization("OPORTUNIDADES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetOportunidadByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetOportunidadByCode")
            .RequireAuthorization("OPORTUNIDADES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateOportunidadCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateOportunidad")
            .RequireAuthorization("OPORTUNIDADES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleOportunidadStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleOportunidadStatus")
            .RequireAuthorization("OPORTUNIDADES.UPDATE");
        }
    }
}
