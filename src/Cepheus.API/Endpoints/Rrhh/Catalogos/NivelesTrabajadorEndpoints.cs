using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.CreateNivelTrabajador;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.GetNivelTrabajadorByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.GetNivelesTrabajadorPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.ToggleNivelTrabajadorStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.UpdateNivelTrabajador;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class NivelesTrabajadorEndpoints
    {
        public static void MapNivelesTrabajadorEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/niveles-trabajador")
                .WithTags("NivelesTrabajador (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateNivelTrabajadorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/niveles-trabajador/{result.Code}", result);
            })
            .WithName("CreateNivelTrabajador")
            .RequireAuthorization("NIVELESTRABAJADOR.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetNivelesTrabajadorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNivelesTrabajadorPagedQueryString")
            .RequireAuthorization("NIVELESTRABAJADOR.VIEW");

            group.MapPost("/paged/body", async (
                GetNivelesTrabajadorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNivelesTrabajadorPagedBody")
            .RequireAuthorization("NIVELESTRABAJADOR.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetNivelTrabajadorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetNivelTrabajadorByCode")
            .RequireAuthorization("NIVELESTRABAJADOR.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateNivelTrabajadorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateNivelTrabajador")
            .RequireAuthorization("NIVELESTRABAJADOR.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleNivelTrabajadorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleNivelTrabajadorStatus")
            .RequireAuthorization("NIVELESTRABAJADOR.UPDATE");
        }
    }
}