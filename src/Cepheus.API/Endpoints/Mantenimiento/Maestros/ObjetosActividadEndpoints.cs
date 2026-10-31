using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.CreateObjetoActividad;
using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.GetObjetoActividadByCode;
using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.GetObjetosActividadPaginated;
using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.ToggleObjetoActividadStatus;
using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.UpdateObjetoActividad;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.ExportObjetosActividad;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Maestros
{
    public static class ObjetosActividadEndpoints
    {
        public static void MapObjetosActividadEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/maestros/objetos-actividad")
                .WithTags("Objetos de Actividad (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateObjetoActividadCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/maestros/objetos-actividad/{result.Code}", result);
            })
            .WithName("CreateObjetoActividad")
            .RequireAuthorization("OBJETOSACTIVIDAD.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetObjetosActividadPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetObjetosActividadPagedQueryString")
            .RequireAuthorization("OBJETOSACTIVIDAD.VIEW");

            group.MapPost("/paged/body", async (
                GetObjetosActividadPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetObjetosActividadPagedBody")
            .RequireAuthorization("OBJETOSACTIVIDAD.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportObjetosActividadQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportObjetosActividadExcel")
            .RequireAuthorization("OBJETOSACTIVIDAD.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportObjetosActividadQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportObjetosActividadPdf")
            .RequireAuthorization("OBJETOSACTIVIDAD.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetObjetoActividadByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetObjetoActividadByCode")
            .RequireAuthorization("OBJETOSACTIVIDAD.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateObjetoActividadCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateObjetoActividad")
            .RequireAuthorization("OBJETOSACTIVIDAD.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleObjetoActividadStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleObjetoActividadStatus")
            .RequireAuthorization("OBJETOSACTIVIDAD.UPDATE");
        }
    }
}
