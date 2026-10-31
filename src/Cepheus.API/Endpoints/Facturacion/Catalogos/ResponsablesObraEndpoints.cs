using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.CreateResponsableObra;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.DeleteResponsableObra;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.GetResponsableObra;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.GetResponsablesObraPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.UpdateResponsableObra;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.ExportResponsablesObra;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class ResponsablesObraEndpoints
    {
        public static void MapResponsablesObraEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/responsables-obra")
                .WithTags("Responsables de Obra (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateResponsableObraCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/catalogos/responsables-obra/{result.ClienteCode}/{result.ObraCode}/{result.Code}",
                    result);
            })
            .WithName("CreateResponsableObra")
            .RequireAuthorization("OBRAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetResponsablesObraPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetResponsablesObraPagedQueryString")
            .RequireAuthorization("OBRAS.VIEW");

            group.MapPost("/paged/body", async (
                GetResponsablesObraPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetResponsablesObraPagedBody")
            .RequireAuthorization("OBRAS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportResponsablesObraQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportResponsablesObraExcel")
            .RequireAuthorization("OBRAS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportResponsablesObraQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportResponsablesObraPdf")
            .RequireAuthorization("OBRAS.EXPORT_PDF");

            group.MapGet("/{clienteCode}/{obraCode}/{code:int}", async (
                string clienteCode, string obraCode, int code,
                ISender sender) =>
            {
                var result = await sender.Send(new GetResponsableObraQuery(clienteCode, obraCode, code));
                return Results.Ok(result);
            })
            .WithName("GetResponsableObra")
            .RequireAuthorization("OBRAS.VIEW");

            group.MapPut("/{clienteCode}/{obraCode}/{code:int}", async (
                string clienteCode, string obraCode, int code,
                UpdateResponsableObraCommand command, ISender sender) =>
            {
                if (clienteCode != command.ClienteCode || obraCode != command.ObraCode || code != command.Code)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateResponsableObra")
            .RequireAuthorization("OBRAS.UPDATE");

            group.MapDelete("/{clienteCode}/{obraCode}/{code:int}", async (
                string clienteCode, string obraCode, int code,
                ISender sender) =>
            {
                await sender.Send(new DeleteResponsableObraCommand(clienteCode, obraCode, code));
                return Results.NoContent();
            })
            .WithName("DeleteResponsableObra")
            .RequireAuthorization("OBRAS.DELETE");
        }
    }
}
