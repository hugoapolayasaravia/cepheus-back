using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.CreateEspecialidad;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.GetEspecialidadByCode;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.GetEspecialidadesPaginated;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.ToggleEspecialidadStatus;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.UpdateEspecialidad;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.ExportEspecialidades;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Catalogos
{
    public static class EspecialidadesEndpoints
    {
        public static void MapEspecialidadesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/catalogos/especialidades")
                .WithTags("Especialidades (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateEspecialidadCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/catalogos/especialidades/{result.Code}", result);
            })
            .WithName("CreateEspecialidad")
            .RequireAuthorization("ESPECIALIDADES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetEspecialidadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEspecialidadesPagedQueryString")
            .RequireAuthorization("ESPECIALIDADES.VIEW");

            group.MapPost("/paged/body", async (
                GetEspecialidadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEspecialidadesPagedBody")
            .RequireAuthorization("ESPECIALIDADES.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportEspecialidadesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportEspecialidadesExcel")
            .RequireAuthorization("ESPECIALIDADES.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportEspecialidadesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportEspecialidadesPdf")
            .RequireAuthorization("ESPECIALIDADES.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetEspecialidadByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetEspecialidadByCode")
            .RequireAuthorization("ESPECIALIDADES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateEspecialidadCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateEspecialidad")
            .RequireAuthorization("ESPECIALIDADES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleEspecialidadStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleEspecialidadStatus")
            .RequireAuthorization("ESPECIALIDADES.UPDATE");
        }
    }
}
