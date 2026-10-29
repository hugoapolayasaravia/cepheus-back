using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.CreateMaquina;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.GetMaquinaByCode;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.GetMaquinasPaginated;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.ToggleMaquinaStatus;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.UpdateMaquina;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Catalogos
{
    public static class MaquinasEndpoints
    {
        public static void MapMaquinasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/catalogos/maquinas")
                .WithTags("Máquinas (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateMaquinaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/catalogos/maquinas/{result.Code}", result);
            })
            .WithName("CreateMaquina")
            .RequireAuthorization("MAQUINAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetMaquinasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetMaquinasPagedQueryString")
            .RequireAuthorization("MAQUINAS.VIEW");

            group.MapPost("/paged/body", async (
                GetMaquinasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetMaquinasPagedBody")
            .RequireAuthorization("MAQUINAS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetMaquinaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetMaquinaByCode")
            .RequireAuthorization("MAQUINAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateMaquinaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateMaquina")
            .RequireAuthorization("MAQUINAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleMaquinaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleMaquinaStatus")
            .RequireAuthorization("MAQUINAS.UPDATE");
        }
    }
}
