using Cepheus.Application.Features.Comunes.Plantas.CreatePlanta;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Plantas.ExportPlantas;
using Cepheus.Application.Features.Comunes.Plantas.GetPlantaById;
using Cepheus.Application.Features.Comunes.Plantas.GetPlantasPaginated;
using Cepheus.Application.Features.Comunes.Plantas.TogglePlantaStatus;
using Cepheus.Application.Features.Comunes.Plantas.UpdatePlanta;
using MediatR;

namespace Cepheus.API.Endpoints.Comunes
{
    public static class PlantasEndpoints
    {
        public static void MapPlantasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/comunes/plantas")
                .WithTags("Plantas")
                .RequireAuthorization();

            group.MapPost("/", async (CreatePlantaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/comunes/plantas/{result.Code}", result);
            })
            .WithName("CreatePlanta")
            .RequireAuthorization("PLANTAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetPlantasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPlantasPagedQueryString") 
            .RequireAuthorization("PLANTAS.VIEW");

            group.MapPost("/paged/body", async (
                GetPlantasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPlantasPagedBody")
            .RequireAuthorization("PLANTAS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportPlantasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportPlantasExcel")
            .RequireAuthorization("PLANTAS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportPlantasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportPlantasPdf")
            .RequireAuthorization("PLANTAS.EXPORT_PDF");

            group.MapGet("/{code}", async (string code, ISender sender) =>
            {
                var result = await sender.Send(new GetPlantaByIdQuery(code));
                return Results.Ok(result);
            })
            .WithName("GetPlantaById")
            .RequireAuthorization("PLANTAS.VIEW");

            group.MapPut("/{code}", async (string code, UpdatePlantaCommand command, ISender sender) =>
            {
                if (!string.Equals(
                    code,
                    command.Code,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest(
                        "El código de la ruta no coincide con el código del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdatePlanta")
            .RequireAuthorization("PLANTAS.UPDATE");

            group.MapPatch("/{code}/toggle-status", async (string code, ISender sender) =>
            {
                var isActive = await sender.Send(new TogglePlantaStatusCommand(code));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("TogglePlantaStatus")
            .RequireAuthorization("PLANTAS.UPDATE");
        }
    }
}
