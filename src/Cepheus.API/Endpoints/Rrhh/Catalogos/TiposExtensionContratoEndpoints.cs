using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.CreateTipoExtensionContrato;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.GetTipoExtensionContratoByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.GetTiposExtensionContratoPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.ToggleTipoExtensionContratoStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.UpdateTipoExtensionContrato;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposExtensionContratoEndpoints
    {
        public static void MapTiposExtensionContratoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-extension-contrato")
                .WithTags("TiposExtensionContrato (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoExtensionContratoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-extension-contrato/{result.Code}", result);
            })
            .WithName("CreateTipoExtensionContrato")
            .RequireAuthorization("TIPOSEXTENSIONCONTRA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposExtensionContratoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposExtensionContratoPagedQueryString")
            .RequireAuthorization("TIPOSEXTENSIONCONTRA.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposExtensionContratoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposExtensionContratoPagedBody")
            .RequireAuthorization("TIPOSEXTENSIONCONTRA.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoExtensionContratoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoExtensionContratoByCode")
            .RequireAuthorization("TIPOSEXTENSIONCONTRA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoExtensionContratoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoExtensionContrato")
            .RequireAuthorization("TIPOSEXTENSIONCONTRA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoExtensionContratoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoExtensionContratoStatus")
            .RequireAuthorization("TIPOSEXTENSIONCONTRA.UPDATE");
        }
    }
}