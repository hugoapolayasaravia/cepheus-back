using Cepheus.Application.Features.Logistica.Maestros.Proveedores.CreateProveedor;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Proveedores.ExportProveedores;
using Cepheus.Application.Features.Logistica.Maestros.Proveedores.GetProveedorByCode;
using Cepheus.Application.Features.Logistica.Maestros.Proveedores.GetProveedoresPaginated;
using Cepheus.Application.Features.Logistica.Maestros.Proveedores.ToggleProveedorStatus;
using Cepheus.Application.Features.Logistica.Maestros.Proveedores.UpdateProveedor;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ProveedoresEndpoints
    {
        public static void MapProveedoresEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/proveedores")
                .WithTags("Proveedores (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateProveedorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/proveedores/{result.Code}", result);
            })
            .WithName("CreateProveedor")
            .RequireAuthorization("PROVEEDORES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetProveedoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetProveedoresPagedQueryString")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapPost("/paged/body", async (
                GetProveedoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetProveedoresPagedBody")
            .RequireAuthorization("PROVEEDORES.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportProveedoresQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportProveedoresExcel")
            .RequireAuthorization("PROVEEDORES.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportProveedoresQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportProveedoresPdf")
            .RequireAuthorization("PROVEEDORES.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetProveedorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetProveedorByCode")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateProveedorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateProveedor")
            .RequireAuthorization("PROVEEDORES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleProveedorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleProveedorStatus")
            .RequireAuthorization("PROVEEDORES.UPDATE");
        }
    }
}
