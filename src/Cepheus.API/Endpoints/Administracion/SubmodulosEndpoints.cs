using Cepheus.Application.Features.Administracion.Submodulos.ExportSubmodulos;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Submodulos.CreateSubmodulo;
using Cepheus.Application.Features.Administracion.Submodulos.GetSubmoduloById;
using Cepheus.Application.Features.Administracion.Submodulos.GetSubmodulosPaginated;
using Cepheus.Application.Features.Administracion.Submodulos.ToggleSubmoduloStatus;
using Cepheus.Application.Features.Administracion.Submodulos.UpdateSubmodulo;
using MediatR;

namespace Cepheus.API.Endpoints.Administracion
{
    public static class SubmodulosEndpoints
    {
        public static void MapSubmodulosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/submodulos")
                .WithTags("Submodulos")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSubmoduloCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/submodulos/{result.Id}", result);
            })
            .WithName("CreateSubmodulo")
            .RequireAuthorization("SUBMODULOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSubmodulosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSubmodulosPagedQueryString")
            .RequireAuthorization("SUBMODULOS.VIEW");

            group.MapPost("/paged/body", async (
                GetSubmodulosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSubmodulosPagedBody")
            .RequireAuthorization("SUBMODULOS.VIEW");

            // GET /api/submodulos/export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportSubmodulosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportSubmodulosExcel")
            .RequireAuthorization("SUBMODULOS.EXPORT_EXCEL");

            // GET /api/submodulos/export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportSubmodulosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportSubmodulosPdf")
            .RequireAuthorization("SUBMODULOS.EXPORT_PDF");

            group.MapGet("/{id:int}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetSubmoduloByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetSubmoduloById")
            .RequireAuthorization("SUBMODULOS.VIEW");

            group.MapPut("/{id:int}", async (int id, UpdateSubmoduloCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El Id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSubmodulo")
            .RequireAuthorization("SUBMODULOS.UPDATE");

            group.MapPatch("/{id:int}/toggle-status", async (int id, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleSubmoduloStatusCommand(id));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleSubmoduloStatus")
            .RequireAuthorization("SUBMODULOS.UPDATE");
        }
    }


}
