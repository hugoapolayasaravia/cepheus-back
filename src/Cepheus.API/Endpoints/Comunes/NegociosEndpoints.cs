using Cepheus.Application.Features.Comunes.Negocios.CreateNegocio;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Negocios.ExportNegocios;
using Cepheus.Application.Features.Comunes.Negocios.GetNegocioByCode;
using Cepheus.Application.Features.Comunes.Negocios.GetNegociosPaginated;
using Cepheus.Application.Features.Comunes.Negocios.ToggleNegocioStatus;
using Cepheus.Application.Features.Comunes.Negocios.UpdateNegocio;
using MediatR;

namespace Cepheus.API.Endpoints.Comun
{
    public static class NegociosEndpoints
    {
        public static void MapNegociosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/comun/negocios")
                .WithTags("Negocios (Común)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateNegocioCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/comun/negocios/{result.Code}", result);
            })
            .WithName("CreateNegocio")
            .RequireAuthorization("NEGOCIOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetNegociosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNegociosPagedQueryString")
            .RequireAuthorization("NEGOCIOS.VIEW");

            group.MapPost("/paged/body", async (
                GetNegociosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNegociosPagedBody")
            .RequireAuthorization("NEGOCIOS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportNegociosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportNegociosExcel")
            .RequireAuthorization("NEGOCIOS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportNegociosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportNegociosPdf")
            .RequireAuthorization("NEGOCIOS.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetNegocioByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetNegocioByCode")
            .RequireAuthorization("NEGOCIOS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateNegocioCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateNegocio")
            .RequireAuthorization("NEGOCIOS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleNegocioStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleNegocioStatus")
            .RequireAuthorization("NEGOCIOS.UPDATE");
        }
    }
}
