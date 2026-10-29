using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.CreateUnidadMedida;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.ExportUnidadesMedida;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.GetUnidadMedidaByCode;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.GetUnidadesMedidaPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.ToggleUnidadMedidaStatus;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.UpdateUnidadMedida;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class UnidadesMedidaEndpoints
    {
        public static void MapUnidadesMedidaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/unidades-medida")
                .WithTags("Unidades de Medida (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateUnidadMedidaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/unidades-medida/{result.Code}", result);
            })
            .WithName("CreateUnidadMedida")
            .RequireAuthorization("UNIDADESMEDIDA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetUnidadesMedidaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUnidadesMedidaPagedQueryString")
            .RequireAuthorization("UNIDADESMEDIDA.VIEW");

            group.MapPost("/paged/body", async (
                GetUnidadesMedidaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUnidadesMedidaPagedBody")
            .RequireAuthorization("UNIDADESMEDIDA.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportUnidadesMedidaQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportUnidadesMedidaExcel")
            .RequireAuthorization("UNIDADESMEDIDA.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportUnidadesMedidaQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportUnidadesMedidaPdf")
            .RequireAuthorization("UNIDADESMEDIDA.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetUnidadMedidaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetUnidadMedidaByCode")
            .RequireAuthorization("UNIDADESMEDIDA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateUnidadMedidaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateUnidadMedida")
            .RequireAuthorization("UNIDADESMEDIDA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleUnidadMedidaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleUnidadMedidaStatus")
            .RequireAuthorization("UNIDADESMEDIDA.UPDATE");
        }
    }
}