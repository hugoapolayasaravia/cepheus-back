using Cepheus.Application.Features.Comunes.Ubigeos.CreateUbigeo;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Ubigeos.ExportUbigeos;
using Cepheus.Application.Features.Comunes.Ubigeos.GetUbigeoById;
using Cepheus.Application.Features.Comunes.Ubigeos.GetUbigeosPaginated;
using Cepheus.Application.Features.Comunes.Ubigeos.ToggleUbigeoStatus;
using Cepheus.Application.Features.Comunes.Ubigeos.UpdateUbigeo;
using MediatR;

namespace Cepheus.API.Endpoints.Comunes
{
    public static class UbigeosEndpoints
    {
        public static void MapUbigeosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/comunes/ubigeos")
                .WithTags("Ubigeos")
                .RequireAuthorization();

            group.MapPost("/", async (CreateUbigeoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/comunes/ubigeos/{result.Code}", result);
            })
            .WithName("CreateUbigeo")
            .RequireAuthorization("UBIGEOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetUbigeosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUbigeosPagedQueryString")
            .RequireAuthorization("UBIGEOS.VIEW");

            group.MapPost("/paged/body", async (
                GetUbigeosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUbigeosPagedBody")
            .RequireAuthorization("UBIGEOS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportUbigeosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportUbigeosExcel")
            .RequireAuthorization("UBIGEOS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportUbigeosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportUbigeosPdf")
            .RequireAuthorization("UBIGEOS.EXPORT_PDF");

            group.MapGet("/{code}", async (string code, ISender sender) =>
            {
                var result = await sender.Send(new GetUbigeoByIdQuery(code));
                return Results.Ok(result);
            })
            .WithName("GetUbigeoById")
            .RequireAuthorization("UBIGEOS.VIEW");

            group.MapPut("/{code}", async (string code, UpdateUbigeoCommand command, ISender sender) =>
            {
                if(!string.Equals(
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
            .WithName("UpdateUbigeo")
            .RequireAuthorization("UBIGEOS.UPDATE");

            group.MapPatch("/{code}/toggle-status", async (string code, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleUbigeoStatusCommand(code));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleUbigeoStatus")
            .RequireAuthorization("UBIGEOS.UPDATE");
        }
    }
}
