using Cepheus.Application.Features.Logistica.Catalogos.Tramites.CreateTramite;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.Tramites.ExportTramites;
using Cepheus.Application.Features.Logistica.Catalogos.Tramites.GetTramiteByCode;
using Cepheus.Application.Features.Logistica.Catalogos.Tramites.GetTramitesPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.Tramites.ToggleTramiteStatus;
using Cepheus.Application.Features.Logistica.Catalogos.Tramites.UpdateTramite;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class TramitesEndpoints
    {
        public static void MapTramitesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/tramites")
                .WithTags("Trámites (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTramiteCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/tramites/{result.Code}", result);
            })
            .WithName("CreateTramite")
            .RequireAuthorization("TRAMITES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTramitesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTramitesPagedQueryString")
            .RequireAuthorization("TRAMITES.VIEW");

            group.MapPost("/paged/body", async (
                GetTramitesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTramitesPagedBody")
            .RequireAuthorization("TRAMITES.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTramitesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTramitesExcel")
            .RequireAuthorization("TRAMITES.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTramitesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTramitesPdf")
            .RequireAuthorization("TRAMITES.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTramiteByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTramiteByCode")
            .RequireAuthorization("TRAMITES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTramiteCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTramite")
            .RequireAuthorization("TRAMITES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTramiteStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTramiteStatus")
            .RequireAuthorization("TRAMITES.UPDATE");
        }
    }
}