using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.CreateAnexo;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.GetAnexoByCode;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.GetAnexosPaginated;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.ToggleAnexoStatus;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.UpdateAnexo;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Maestros
{
    public static class AnexosEndpoints
    {
        public static void MapAnexosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/maestros/anexos")
                .WithTags("Anexos contables")
                .RequireAuthorization();

            group.MapPost("/", async (CreateAnexoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/maestros/anexos/{result.TipoAnexoCode}/{result.Code}", result);
            })
            .WithName("CreateAnexo")
            .RequireAuthorization("ANEXOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetAnexosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAnexosPagedQueryString")
            .RequireAuthorization("ANEXOS.VIEW");

            group.MapPost("/paged/body", async (
                GetAnexosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAnexosPagedBody")
            .RequireAuthorization("ANEXOS.VIEW");

            group.MapGet("/{tipoAnexoCode}/{codigo}", async (string tipoAnexoCode, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetAnexoByCodeQuery(tipoAnexoCode, codigo));
                return Results.Ok(result);
            })
            .WithName("GetAnexoByCode")
            .RequireAuthorization("ANEXOS.VIEW");

            group.MapPut("/{tipoAnexoCode}/{codigo}", async (string tipoAnexoCode, string codigo, UpdateAnexoCommand command, ISender sender) =>
            {
                if (!string.Equals(tipoAnexoCode, command.TipoAnexoCode, StringComparison.OrdinalIgnoreCase) || !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateAnexo")
            .RequireAuthorization("ANEXOS.UPDATE");

            group.MapPatch("/{tipoAnexoCode}/{codigo}/toggle-status", async (string tipoAnexoCode, string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleAnexoStatusCommand(tipoAnexoCode, codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleAnexoStatus")
            .RequireAuthorization("ANEXOS.UPDATE");
        }
    }
}
