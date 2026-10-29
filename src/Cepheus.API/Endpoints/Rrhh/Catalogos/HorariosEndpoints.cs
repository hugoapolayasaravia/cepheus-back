using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.CreateHorario;
using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.GetHorarioByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.GetHorariosPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.ToggleHorarioStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.UpdateHorario;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class HorariosEndpoints
    {
        public static void MapHorariosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/horarios")
                .WithTags("Horarios (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateHorarioCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/horarios/{result.Code}", result);
            })
            .WithName("CreateHorario")
            .RequireAuthorization("HORARIOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetHorariosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetHorariosPagedQueryString")
            .RequireAuthorization("HORARIOS.VIEW");

            group.MapPost("/paged/body", async (
                GetHorariosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetHorariosPagedBody")
            .RequireAuthorization("HORARIOS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetHorarioByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetHorarioByCode")
            .RequireAuthorization("HORARIOS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateHorarioCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateHorario")
            .RequireAuthorization("HORARIOS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleHorarioStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleHorarioStatus")
            .RequireAuthorization("HORARIOS.UPDATE");
        }
    }
}