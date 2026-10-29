using Cepheus.Application.Features.Administracion.Programas.ExportProgramas;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Programas.CreatePrograma;
using Cepheus.Application.Features.Administracion.Programas.GetProgramaById;
using Cepheus.Application.Features.Administracion.Programas.GetProgramasPaginated;
using Cepheus.Application.Features.Administracion.Programas.ToggleProgramaStatus;
using Cepheus.Application.Features.Administracion.Programas.UpdatePrograma;
using MediatR;

namespace Cepheus.API.Endpoints.Administracion
{
    public static class ProgramasEndpoints
    {
        public static void MapProgramasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/programas")
                .WithTags("Programas")
                .RequireAuthorization();

            group.MapPost("/", async (CreateProgramaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/programas/{result.Id}", result);
            })
            .WithName("CreatePrograma")
            .RequireAuthorization("PROGRAMAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetProgramasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetProgramasPagedQueryString")
            .RequireAuthorization("PROGRAMAS.VIEW");

            group.MapPost("/paged/body", async (
                GetProgramasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetProgramasPagedBody")
            .RequireAuthorization("PROGRAMAS.VIEW");

            // GET /api/programas/export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportProgramasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportProgramasExcel")
            .RequireAuthorization("PROGRAMAS.EXPORT_EXCEL");

            // GET /api/programas/export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportProgramasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportProgramasPdf")
            .RequireAuthorization("PROGRAMAS.EXPORT_PDF");

            group.MapGet("/{id:int}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetProgramaByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetProgramaById")
            .RequireAuthorization("PROGRAMAS.VIEW");

            group.MapPut("/{id:int}", async (int id, UpdateProgramaCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El Id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdatePrograma")
            .RequireAuthorization("PROGRAMAS.UPDATE");

            group.MapPatch("/{id:int}/toggle-status", async (int id, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleProgramaStatusCommand(id));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleProgramaStatus")
            .RequireAuthorization("PROGRAMAS.UPDATE");
        }
    }


}
