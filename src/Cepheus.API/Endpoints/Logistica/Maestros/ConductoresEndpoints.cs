using Cepheus.Application.Features.Logistica.Maestros.Conductores.CreateConductor;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.GetConductorByCode;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.GetConductoresPaginated;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.ToggleConductorStatus;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.UpdateConductor;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.ExportConductores;
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

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportConductoresQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportConductoresExcel")
            .RequireAuthorization("CONDUCTORES.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportConductoresQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportConductoresPdf")
            .RequireAuthorization("CONDUCTORES.EXPORT_PDF");

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
