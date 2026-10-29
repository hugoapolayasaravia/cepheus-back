// Cepheus.API/Endpoints/Logistica/Transacciones/ImportacionesEndpoints.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.ChangeEstadoImportacion;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.CreateImportacion;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.CreateImportacionDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.CreateImportacionGasto;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.DeleteImportacionGasto;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.RecalcularProrrateoImportacion;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacionGasto;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.DeleteImportacionDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacionDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GenerarNotaIngresoImportacion;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionByCode;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionNotasIngreso;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionPendientes;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionesPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacion;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones
{
    public static class ImportacionesEndpoints
    {
        public static void MapImportacionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/transacciones/importaciones")
                .WithTags("Importaciones (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateImportacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/transacciones/importaciones/{result.PlantaCode}/{result.Code}", result);
            })
            .WithName("CreateImportacion")
            .RequireAuthorization("IMPORTACIONES.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetImportacionesPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetImportacionesPagedQueryString")
            .RequireAuthorization("IMPORTACIONES.VIEW");

            group.MapPost("/paged/body", async (GetImportacionesPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetImportacionesPagedBody")
            .RequireAuthorization("IMPORTACIONES.VIEW");

            group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new GetImportacionByCodeQuery(codigoPlanta, codigo))))
            .WithName("GetImportacionByCode")
            .RequireAuthorization("IMPORTACIONES.VIEW");

            group.MapPut("/{codigoPlanta}/{codigo}", async (
                string codigoPlanta, string codigo, UpdateImportacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateImportacion")
            .RequireAuthorization("IMPORTACIONES.UPDATE");

            group.MapPatch("/{codigoPlanta}/{codigo}/estado", async (
                string codigoPlanta, string codigo, ChangeEstadoBody body, ISender sender) =>
                Results.Ok(await sender.Send(new ChangeEstadoImportacionCommand(codigoPlanta, codigo, body.NuevoEstado))))
            .WithName("ChangeEstadoImportacion")
            .RequireAuthorization("IMPORTACIONES.UPDATE");

            group.MapPost("/{codigoPlanta}/{codigoImportacion}/detalles", async (
                string codigoPlanta, string codigoImportacion, CreateImportacionDetalleCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoImportacion, command.ImportacionCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("CreateImportacionDetalle")
            .RequireAuthorization("IMPORTACIONES.UPDATE");

            group.MapPut("/{codigoPlanta}/{codigoImportacion}/detalles/{codigoProveedor}/{codigoArticulo}", async (
                string codigoPlanta, string codigoImportacion, string codigoProveedor, string codigoArticulo,
                UpdateImportacionDetalleCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoImportacion, command.ImportacionCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoProveedor, command.ProveedorCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoArticulo, command.ArticuloCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateImportacionDetalle")
            .RequireAuthorization("IMPORTACIONES.UPDATE");

            group.MapDelete("/{codigoPlanta}/{codigoImportacion}/detalles/{codigoProveedor}/{codigoArticulo}", async (
                string codigoPlanta, string codigoImportacion, string codigoProveedor, string codigoArticulo, ISender sender) =>
                Results.Ok(await sender.Send(new DeleteImportacionDetalleCommand(
                    codigoPlanta, codigoImportacion, codigoProveedor, codigoArticulo))))
            .WithName("DeleteImportacionDetalle")
            .RequireAuthorization("IMPORTACIONES.UPDATE");

            group.MapPost("/{codigoPlanta}/{codigoImportacion}/gastos", async (
                string codigoPlanta, string codigoImportacion, CreateImportacionGastoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoImportacion, command.ImportacionCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("CreateImportacionGasto")
            .RequireAuthorization("IMPORTACIONES.UPDATE");

            group.MapPut("/{codigoPlanta}/{codigoImportacion}/gastos/{codigoProveedor}/{numeroDocumento}", async (
                string codigoPlanta, string codigoImportacion, string codigoProveedor, string numeroDocumento,
                UpdateImportacionGastoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoImportacion, command.ImportacionCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoProveedor, command.ProveedorCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(numeroDocumento, command.NumeroDocumento, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateImportacionGasto")
            .RequireAuthorization("IMPORTACIONES.UPDATE");

            group.MapDelete("/{codigoPlanta}/{codigoImportacion}/gastos/{codigoProveedor}/{numeroDocumento}", async (
                string codigoPlanta, string codigoImportacion, string codigoProveedor, string numeroDocumento, ISender sender) =>
                Results.Ok(await sender.Send(new DeleteImportacionGastoCommand(
                    codigoPlanta, codigoImportacion, codigoProveedor, numeroDocumento))))
            .WithName("DeleteImportacionGasto")
            .RequireAuthorization("IMPORTACIONES.UPDATE");

            group.MapPost("/{codigoPlanta}/{codigoImportacion}/prorrateo", async (
                string codigoPlanta, string codigoImportacion, ISender sender) =>
                Results.Ok(await sender.Send(new RecalcularProrrateoImportacionCommand(codigoPlanta, codigoImportacion))))
            .WithName("RecalcularProrrateoImportacion")
            .RequireAuthorization("IMPORTACIONES.UPDATE");

            group.MapGet("/{codigoPlanta}/{codigoImportacion}/pendientes", async (
                string codigoPlanta, string codigoImportacion, string? proveedorCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetImportacionPendientesQuery(codigoPlanta, codigoImportacion, proveedorCode))))
            .WithName("GetImportacionPendientes")
            .RequireAuthorization("IMPORTACIONES.VIEW");

            group.MapGet("/{codigoPlanta}/{codigoImportacion}/notas-ingreso", async (
                string codigoPlanta, string codigoImportacion, ISender sender) =>
                Results.Ok(await sender.Send(new GetImportacionNotasIngresoQuery(codigoPlanta, codigoImportacion))))
            .WithName("GetImportacionNotasIngreso")
            .RequireAuthorization("IMPORTACIONES.VIEW");

            // Genera la Nota de Ingreso de un proveedor e ingresa el stock (legacy: Generar, opción 1).
            group.MapPost("/{codigoPlanta}/{codigoImportacion}/generar-nota-ingreso", async (
                string codigoPlanta, string codigoImportacion, GenerarNotaIngresoImportacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoImportacion, command.ImportacionCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("GenerarNotaIngresoImportacion")
            .RequireAuthorization("IMPORTACIONES.UPDATE");
        }

        public record ChangeEstadoBody(string NuevoEstado);
    }
}
