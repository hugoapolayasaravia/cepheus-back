using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.CreateTipoCuentaContable;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.GetTipoCuentaContableById;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.GetTiposCuentaContablePaginated;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.ToggleTipoCuentaContableStatus;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.UpdateTipoCuentaContable;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.ExportTiposCuentaContable;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Catalogos
{
    public static class TiposCuentaContableEndpoints
    {
        public static void MapTiposCuentaContableEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/catalogos/tipos-cuenta-contable")
                .WithTags("Tipos de cuenta contable")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoCuentaContableCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/catalogos/tipos-cuenta-contable/{result.Code}", result);
            })
            .WithName("CreateTipoCuentaContable")
            .RequireAuthorization("TIPOSCUENTACONTABLE.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposCuentaContablePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposCuentaContablePagedQueryString")
            .RequireAuthorization("TIPOSCUENTACONTABLE.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposCuentaContablePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposCuentaContablePagedBody")
            .RequireAuthorization("TIPOSCUENTACONTABLE.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposCuentaContableQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposCuentaContableExcel")
            .RequireAuthorization("TIPOSCUENTACONTABLE.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposCuentaContableQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposCuentaContablePdf")
            .RequireAuthorization("TIPOSCUENTACONTABLE.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoCuentaContableByIdQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoCuentaContableById")
            .RequireAuthorization("TIPOSCUENTACONTABLE.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoCuentaContableCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoCuentaContable")
            .RequireAuthorization("TIPOSCUENTACONTABLE.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoCuentaContableStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoCuentaContableStatus")
            .RequireAuthorization("TIPOSCUENTACONTABLE.UPDATE");
        }
    }
}
