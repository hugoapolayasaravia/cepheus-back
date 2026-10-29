using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.CreateNacionalidad;
using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.GetNacionalidadByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.GetNacionalidadesPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.ToggleNacionalidadStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.UpdateNacionalidad;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class NacionalidadesEndpoints
    {
        public static void MapNacionalidadesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/nacionalidades")
                .WithTags("Nacionalidades (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateNacionalidadCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/nacionalidades/{result.Code}", result);
            })
            .WithName("CreateNacionalidad")
            .RequireAuthorization("NACIONALIDADES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetNacionalidadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNacionalidadesPagedQueryString")
            .RequireAuthorization("NACIONALIDADES.VIEW");

            group.MapPost("/paged/body", async (
                GetNacionalidadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNacionalidadesPagedBody")
            .RequireAuthorization("NACIONALIDADES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetNacionalidadByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetNacionalidadByCode")
            .RequireAuthorization("NACIONALIDADES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateNacionalidadCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateNacionalidad")
            .RequireAuthorization("NACIONALIDADES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleNacionalidadStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleNacionalidadStatus")
            .RequireAuthorization("NACIONALIDADES.UPDATE");
        }
    }
}