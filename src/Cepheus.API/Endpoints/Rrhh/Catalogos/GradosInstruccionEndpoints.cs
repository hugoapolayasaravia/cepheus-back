using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.CreateGradoInstruccion;
using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.GetGradoInstruccionByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.GetGradosInstruccionPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.ToggleGradoInstruccionStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.UpdateGradoInstruccion;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class GradosInstruccionEndpoints
    {
        public static void MapGradosInstruccionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/grados-instruccion")
                .WithTags("GradosInstruccion (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateGradoInstruccionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/grados-instruccion/{result.Code}", result);
            })
            .WithName("CreateGradoInstruccion")
            .RequireAuthorization("GRADOSINSTRUCCION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetGradosInstruccionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetGradosInstruccionPagedQueryString")
            .RequireAuthorization("GRADOSINSTRUCCION.VIEW");

            group.MapPost("/paged/body", async (
                GetGradosInstruccionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetGradosInstruccionPagedBody")
            .RequireAuthorization("GRADOSINSTRUCCION.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetGradoInstruccionByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetGradoInstruccionByCode")
            .RequireAuthorization("GRADOSINSTRUCCION.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateGradoInstruccionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateGradoInstruccion")
            .RequireAuthorization("GRADOSINSTRUCCION.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleGradoInstruccionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleGradoInstruccionStatus")
            .RequireAuthorization("GRADOSINSTRUCCION.UPDATE");
        }
    }
}