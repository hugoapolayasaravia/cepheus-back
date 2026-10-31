using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.CreatePlantillaAsiento;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.GetPlantillaAsientoByCode;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.GetPlantillasAsientoPaginated;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.TogglePlantillaAsientoStatus;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.UpdatePlantillaAsiento;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.ExportPlantillasAsiento;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Maestros
{
    public static class PlantillasAsientoEndpoints
    {
        public static void MapPlantillasAsientoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/maestros/plantillas-asiento")
                .WithTags("Plantillas de asiento")
                .RequireAuthorization();

            group.MapPost("/", async (CreatePlantillaAsientoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/maestros/plantillas-asiento/{result.Code}", result);
            })
            .WithName("CreatePlantillaAsiento")
            .RequireAuthorization("PLANTILLASASIENTO.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetPlantillasAsientoPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetPlantillasAsientoPagedQueryString")
            .RequireAuthorization("PLANTILLASASIENTO.VIEW");

            group.MapPost("/paged/body", async (GetPlantillasAsientoPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetPlantillasAsientoPagedBody")
            .RequireAuthorization("PLANTILLASASIENTO.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportPlantillasAsientoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportPlantillasAsientoExcel")
            .RequireAuthorization("PLANTILLASASIENTO.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportPlantillasAsientoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportPlantillasAsientoPdf")
            .RequireAuthorization("PLANTILLASASIENTO.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new GetPlantillaAsientoByCodeQuery(codigo))))
            .WithName("GetPlantillaAsientoByCode")
            .RequireAuthorization("PLANTILLASASIENTO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdatePlantillaAsientoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdatePlantillaAsiento")
            .RequireAuthorization("PLANTILLASASIENTO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new TogglePlantillaAsientoStatusCommand(codigo));
                return Results.Ok(new { isActive });
            })
            .WithName("TogglePlantillaAsientoStatus")
            .RequireAuthorization("PLANTILLASASIENTO.UPDATE");
        }
    }
}
