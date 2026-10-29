using Cepheus.Application.Features.Logistica.Maestros.Conductores.CreateConductor;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.GetConductorByCode;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.GetConductoresPaginated;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.ToggleConductorStatus;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.UpdateConductor;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ConductoresEndpoints
    {
        public static void MapConductoresEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/conductores")
                .WithTags("Conductores (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateConductorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/conductores/{result.Code}", result);
            })
            .WithName("CreateConductor")
            .RequireAuthorization("CONDUCTORES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetConductoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetConductoresPagedQueryString")
            .RequireAuthorization("CONDUCTORES.VIEW");

            group.MapPost("/paged/body", async (
                GetConductoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetConductoresPagedBody")
            .RequireAuthorization("CONDUCTORES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetConductorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetConductorByCode")
            .RequireAuthorization("CONDUCTORES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateConductorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateConductor")
            .RequireAuthorization("CONDUCTORES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleConductorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleConductorStatus")
            .RequireAuthorization("CONDUCTORES.UPDATE");
        }
    }
}
