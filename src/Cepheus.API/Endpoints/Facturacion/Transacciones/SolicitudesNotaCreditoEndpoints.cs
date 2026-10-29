using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.AddSolicitudNotaCreditoArchivo;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.AprobarEtapaSolicitudNotaCredito;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.CreateSolicitudNotaCredito;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.DeleteSolicitudNotaCreditoArchivo;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.GetSolicitudesNotaCreditoPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.GetSolicitudNotaCreditoByCode;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.RechazarEtapaSolicitudNotaCredito;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.UpdateSolicitudNotaCredito;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    public static class SolicitudesNotaCreditoEndpoints
    {
        private const string Route = "/api/facturacion/transacciones/solicitudes-nota-credito";

        public static void MapSolicitudesNotaCreditoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Solicitudes de Nota de Crédito (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSolicitudNotaCreditoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"{Route}/{result.Year}/{result.Month}/{result.Code}", result);
            })
            .WithName("CreateSolicitudNotaCredito")
            .RequireAuthorization("SOLICITUDNC.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSolicitudesNotaCreditoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSolicitudesNotaCreditoPagedQueryString")
            .RequireAuthorization("SOLICITUDNC.VIEW");

            group.MapPost("/paged/body", async (
                GetSolicitudesNotaCreditoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSolicitudesNotaCreditoPagedBody")
            .RequireAuthorization("SOLICITUDNC.VIEW");

            group.MapGet("/{anio}/{mes}/{codigo}", async (string anio, string mes, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetSolicitudNotaCreditoByCodeQuery(anio, mes, codigo));
                return Results.Ok(result);
            })
            .WithName("GetSolicitudNotaCreditoByCode")
            .RequireAuthorization("SOLICITUDNC.VIEW");

            group.MapPut("/{anio}/{mes}/{codigo}", async (
                string anio, string mes, string codigo,
                UpdateSolicitudNotaCreditoCommand command, ISender sender) =>
            {
                if (anio != command.Year || mes != command.Month
                    || !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSolicitudNotaCredito")
            .RequireAuthorization("SOLICITUDNC.UPDATE");

            group.MapPatch("/{anio}/{mes}/{codigo}/etapas/{etapa}/aprobar", async (
                string anio, string mes, string codigo, string etapa,
                EtapaSolicitudNotaCreditoRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(
                    new AprobarEtapaSolicitudNotaCreditoCommand(anio, mes, codigo, etapa, body.Observation));
                return Results.Ok(result);
            })
            .WithName("AprobarEtapaSolicitudNotaCredito")
            .RequireAuthorization("SOLICITUDNC.APPROVE");

            group.MapPatch("/{anio}/{mes}/{codigo}/etapas/{etapa}/rechazar", async (
                string anio, string mes, string codigo, string etapa,
                EtapaSolicitudNotaCreditoRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(
                    new RechazarEtapaSolicitudNotaCreditoCommand(anio, mes, codigo, etapa, body.Observation));
                return Results.Ok(result);
            })
            .WithName("RechazarEtapaSolicitudNotaCredito")
            .RequireAuthorization("SOLICITUDNC.APPROVE");

            group.MapPost("/{anio}/{mes}/{codigo}/archivos", async (
                string anio, string mes, string codigo,
                SolicitudNotaCreditoArchivoRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(
                    new AddSolicitudNotaCreditoArchivoCommand(anio, mes, codigo, body.FileName, body.FilePath));
                return Results.Created($"{Route}/{anio}/{mes}/{codigo}", result);
            })
            .WithName("AddSolicitudNotaCreditoArchivo")
            .RequireAuthorization("SOLICITUDNC.UPDATE");

            group.MapDelete("/{anio}/{mes}/{codigo}/archivos/{nombreArchivo}", async (
                string anio, string mes, string codigo, string nombreArchivo, ISender sender) =>
            {
                await sender.Send(new DeleteSolicitudNotaCreditoArchivoCommand(anio, mes, codigo, nombreArchivo));
                return Results.NoContent();
            })
            .WithName("DeleteSolicitudNotaCreditoArchivo")
            .RequireAuthorization("SOLICITUDNC.UPDATE");
        }
    }

    public record EtapaSolicitudNotaCreditoRequestBody(string? Observation);

    public record SolicitudNotaCreditoArchivoRequestBody(string FileName, string FilePath);
}
