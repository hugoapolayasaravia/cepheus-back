using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.CreateModalidadFormativa;
using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.GetModalidadFormativaByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.GetModalidadesFormativasPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.ToggleModalidadFormativaStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.UpdateModalidadFormativa;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class ModalidadesFormativasEndpoints
    {
        public static void MapModalidadesFormativasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/modalidades-formativas")
                .WithTags("ModalidadesFormativas (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateModalidadFormativaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/modalidades-formativas/{result.Code}", result);
            })
            .WithName("CreateModalidadFormativa")
            .RequireAuthorization("MODALIDADESFORMATIVA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetModalidadesFormativasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetModalidadesFormativasPagedQueryString")
            .RequireAuthorization("MODALIDADESFORMATIVA.VIEW");

            group.MapPost("/paged/body", async (
                GetModalidadesFormativasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetModalidadesFormativasPagedBody")
            .RequireAuthorization("MODALIDADESFORMATIVA.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetModalidadFormativaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetModalidadFormativaByCode")
            .RequireAuthorization("MODALIDADESFORMATIVA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateModalidadFormativaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateModalidadFormativa")
            .RequireAuthorization("MODALIDADESFORMATIVA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleModalidadFormativaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleModalidadFormativaStatus")
            .RequireAuthorization("MODALIDADESFORMATIVA.UPDATE");
        }
    }
}