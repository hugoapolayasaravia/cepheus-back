// Cepheus.API/Endpoints/Logistica/Transacciones/GuiasEndpoints.cs
using Cepheus.Application.Features.Logistica.Transacciones.Guias.ChangeEstadoGuia;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.CreateGuia;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.GetGuiaByCode;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.GetGuiasPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.CreateGuiaDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.DeleteGuiaDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.UpdateGuiaDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.UpdateGuia;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones
{
    public static class GuiasEndpoints
    {
        public static void MapGuiasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/transacciones/guias")
                .WithTags("Guías de Remisión (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateGuiaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/transacciones/guias/{result.PlantaCode}/{result.Code}", result);
            })
            .WithName("CreateGuia")
            .RequireAuthorization("GUIAS.CREATE");

            // Búsqueda paginada: mismos parámetros por query string o por body.
            group.MapGet("/paged", async (
                [AsParameters] GetGuiasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetGuiasPagedQueryString")
            .RequireAuthorization("GUIAS.VIEW");

            group.MapPost("/paged/body", async (
                GetGuiasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetGuiasPagedBody")
            .RequireAuthorization("GUIAS.VIEW");

            group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetGuiaByCodeQuery(codigoPlanta, codigo));
                return Results.Ok(result);
            })
            .WithName("GetGuiaByCode")
            .RequireAuthorization("GUIAS.VIEW");

            group.MapPut("/{codigoPlanta}/{codigo}", async (
                string codigoPlanta, string codigo, UpdateGuiaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateGuia")
            .RequireAuthorization("GUIAS.UPDATE");

            // Anular la guía. Sin toggle-status: el ciclo de vida es
            // Pendiente -> Anulado (ver ChangeEstadoGuiaCommandHandler).
            group.MapPatch("/{codigoPlanta}/{codigo}/estado", async (
                string codigoPlanta, string codigo, ChangeEstadoGuiaBody body, ISender sender) =>
            {
                var result = await sender.Send(new ChangeEstadoGuiaCommand(codigoPlanta, codigo, body.NuevoEstado));
                return Results.Ok(result);
            })
            .WithName("ChangeEstadoGuia")
            .RequireAuthorization("GUIAS.UPDATE");

            // Líneas de detalle: reusan los permisos de Guías. Se identifican
            // por código de artículo (clave legacy), no por número de ítem,
            // porque el ítem se renumera al eliminar una línea.
            group.MapPost("/{codigoPlanta}/{codigo}/detalles", async (
                string codigoPlanta, string codigo, CreateGuiaDetalleBody body, ISender sender) =>
            {
                var command = new CreateGuiaDetalleCommand(
                    codigoPlanta, codigo, body.ArticuloCode, body.Cantidad, body.IsVerified);
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/transacciones/guias/{codigoPlanta}/{codigo}", result);
            })
            .WithName("CreateGuiaDetalle")
            .RequireAuthorization("GUIAS.UPDATE");

            group.MapPut("/{codigoPlanta}/{codigo}/detalles/{articuloCode}", async (
                string codigoPlanta, string codigo, string articuloCode, UpdateGuiaDetalleCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.GuiaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(articuloCode, command.ArticuloCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateGuiaDetalle")
            .RequireAuthorization("GUIAS.UPDATE");

            group.MapDelete("/{codigoPlanta}/{codigo}/detalles/{articuloCode}", async (
                string codigoPlanta, string codigo, string articuloCode, ISender sender) =>
            {
                var result = await sender.Send(new DeleteGuiaDetalleCommand(codigoPlanta, codigo, articuloCode));
                return Results.Ok(result);
            })
            .WithName("DeleteGuiaDetalle")
            .RequireAuthorization("GUIAS.UPDATE");
        }

        public record ChangeEstadoGuiaBody(string NuevoEstado);

        public record CreateGuiaDetalleBody(
            string ArticuloCode,
            decimal Cantidad,
            bool IsVerified);
    }
}
