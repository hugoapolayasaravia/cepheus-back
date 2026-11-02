using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.CreateNoAfectoRetencion;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.GetNoAfectoRetencionByKey;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.GetNoAfectosRetencionPaginated;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.ToggleNoAfectoRetencionStatus;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.UpdateNoAfectoRetencion;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Maestros
{
    public static class NoAfectosRetencionEndpoints
    {
        public static void MapNoAfectosRetencionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/maestros/no-afectos-retencion")
                .WithTags("No afectos a retención")
                .RequireAuthorization();

            group.MapPost("/", async (CreateNoAfectoRetencionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/maestros/no-afectos-retencion/{result.Ruc}/{result.Type}", result);
            })
            .WithName("CreateNoAfectoRetencion")
            .RequireAuthorization("NOAFECTOSRETENCION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetNoAfectosRetencionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNoAfectosRetencionPagedQueryString")
            .RequireAuthorization("NOAFECTOSRETENCION.VIEW");

            group.MapPost("/paged/body", async (
                GetNoAfectosRetencionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNoAfectosRetencionPagedBody")
            .RequireAuthorization("NOAFECTOSRETENCION.VIEW");

            group.MapGet("/{ruc}/{type}", async (string ruc, string type, ISender sender) =>
            {
                var result = await sender.Send(new GetNoAfectoRetencionByKeyQuery(ruc, type));
                return Results.Ok(result);
            })
            .WithName("GetNoAfectoRetencionByKey")
            .RequireAuthorization("NOAFECTOSRETENCION.VIEW");

            group.MapPut("/{ruc}/{type}", async (string ruc, string type, UpdateNoAfectoRetencionCommand command, ISender sender) =>
            {
                if (!string.Equals(ruc, command.Ruc, StringComparison.OrdinalIgnoreCase) || !string.Equals(type, command.Type, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateNoAfectoRetencion")
            .RequireAuthorization("NOAFECTOSRETENCION.UPDATE");

            group.MapPatch("/{ruc}/{type}/toggle-status", async (string ruc, string type, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleNoAfectoRetencionStatusCommand(ruc, type));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleNoAfectoRetencionStatus")
            .RequireAuthorization("NOAFECTOSRETENCION.UPDATE");
        }
    }
}
