using Cepheus.Application.Features.Logistica.Transacciones.Vales.AnulateVale;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.ChangeEstadoVale;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.CreateVale;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.DevolverVale;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValeArticuloInfo;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValeByCode;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValeOrdenTrabajoMateriales;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValesPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.PrepararVale;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.ProcesarVale;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.UpdateVale;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.ValeDetalles.CreateValeDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.ValeDetalles.DeleteValeDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.ValeDetalles.UpdateValeDetalle;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones;

public static class ValesEndpoints
{
    public static void MapValesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/transacciones/vales")
            .WithTags("Vales de Salida (Logística)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateValeCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            return Results.Created(
                $"/api/logistica/transacciones/vales/{result.PlantaCode}/{result.Code}",
                result);
        })
        .WithName("CreateVale")
        .RequireAuthorization("VALESSALIDA.CREATE");

        // Listado: parámetros literales de Logi_sp_Listado_MVales.
        group.MapGet("/paged", async ([AsParameters] GetValesPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetValesPagedQueryString")
        .RequireAuthorization("VALESSALIDA.VIEW");

        group.MapPost("/paged/body", async (GetValesPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetValesPagedBody")
        .RequireAuthorization("VALESSALIDA.VIEW");

        // Saldos, precio y horómetro anterior de un artículo para armar una línea.
        group.MapGet("/articulos/{plantaCode}/{articuloCode}", async (
            string plantaCode,
            string articuloCode,
            string? tipoValeCode,
            string? subCentroCostoCode,
            DateTime? fechaEntrega,
            ISender sender) =>
            Results.Ok(await sender.Send(new GetValeArticuloInfoQuery(
                plantaCode,
                articuloCode,
                tipoValeCode,
                subCentroCostoCode,
                fechaEntrega))))
        .WithName("GetValeArticuloInfo")
        .RequireAuthorization("VALESSALIDA.VIEW");

        // Datos de la OT (en ejecución) y sus materiales pendientes para asignarlos al vale.
        group.MapGet("/ordenes-trabajo/{plantaCode}/{ordenTrabajoCode}/materiales", async (
            string plantaCode,
            string ordenTrabajoCode,
            bool? todos,
            ISender sender) =>
            Results.Ok(await sender.Send(new GetValeOrdenTrabajoMaterialesQuery(
                plantaCode,
                ordenTrabajoCode,
                todos ?? false))))
        .WithName("GetValeOrdenTrabajoMateriales")
        .RequireAuthorization("VALESSALIDA.VIEW");

        group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
            Results.Ok(await sender.Send(new GetValeByCodeQuery(codigoPlanta, codigo))))
        .WithName("GetValeByCode")
        .RequireAuthorization("VALESSALIDA.VIEW");

        group.MapPut("/{codigoPlanta}/{codigo}", async (
            string codigoPlanta,
            string codigo,
            UpdateValeRequest body,
            ISender sender) =>
            Results.Ok(await sender.Send(new UpdateValeCommand(
                codigoPlanta,
                codigo,
                body.FechaEntrega,
                body.SubCentroCostoCode,
                body.SubCentroEjecutorCode,
                body.TrabajadorCode,
                body.UnidadNegocioCode,
                body.PlantaAfectadaCode))))
        .WithName("UpdateVale")
        .RequireAuthorization("VALESSALIDA.UPDATE");

        // ---------------------------------------------------------------- líneas (vale Pendiente)

        group.MapPost("/{codigoPlanta}/{codigo}/detalles", async (
            string codigoPlanta,
            string codigo,
            CreateValeDetalleRequest body,
            ISender sender) =>
            Results.Ok(await sender.Send(new CreateValeDetalleCommand(
                codigoPlanta,
                codigo,
                body.ArticuloCode,
                body.Cantidad,
                body.Propiedad01))))
        .WithName("CreateValeDetalle")
        .RequireAuthorization("VALESSALIDA.UPDATE");

        group.MapPut("/{codigoPlanta}/{codigo}/detalles/{articuloCode}", async (
            string codigoPlanta,
            string codigo,
            string articuloCode,
            UpdateValeDetalleRequest body,
            ISender sender) =>
            Results.Ok(await sender.Send(new UpdateValeDetalleCommand(
                codigoPlanta,
                codigo,
                articuloCode,
                body.Cantidad,
                body.Propiedad01))))
        .WithName("UpdateValeDetalle")
        .RequireAuthorization("VALESSALIDA.UPDATE");

        group.MapDelete("/{codigoPlanta}/{codigo}/detalles/{articuloCode}", async (
            string codigoPlanta,
            string codigo,
            string articuloCode,
            ISender sender) =>
            Results.Ok(await sender.Send(new DeleteValeDetalleCommand(
                codigoPlanta,
                codigo,
                articuloCode))))
        .WithName("DeleteValeDetalle")
        .RequireAuthorization("VALESSALIDA.UPDATE");

        // ---------------------------------------------------------------- flujo

        // Aprobar (Pendiente -> Aprobado) o desaprobar (Aprobado -> Pendiente).
        group.MapPatch("/{codigoPlanta}/{codigo}/estado", async (
            string codigoPlanta,
            string codigo,
            ChangeEstadoBody body,
            ISender sender) =>
            Results.Ok(await sender.Send(new ChangeEstadoValeCommand(
                codigoPlanta,
                codigo,
                body.NuevoEstado))))
        .WithName("ChangeEstadoVale")
        .RequireAuthorization("VALESSALIDA.UPDATE");

        group.MapPost("/{codigoPlanta}/{codigo}/preparar", async (
            string codigoPlanta,
            string codigo,
            ISender sender) =>
            Results.Ok(await sender.Send(new PrepararValeCommand(codigoPlanta, codigo))))
        .WithName("PrepararVale")
        .RequireAuthorization("VALESSALIDA.UPDATE");

        // Articulos vacío = todas las líneas pendientes; con lista = proceso por ítem.
        group.MapPost("/{codigoPlanta}/{codigo}/procesar", async (
            string codigoPlanta,
            string codigo,
            ProcesarValeRequest? body,
            ISender sender) =>
            Results.Ok(await sender.Send(new ProcesarValeCommand(
                codigoPlanta,
                codigo,
                body?.Articulos))))
        .WithName("ProcesarVale")
        .RequireAuthorization("VALESSALIDA.UPDATE");

        // Articulos vacío = todas las líneas procesadas; con lista = devolución por ítem.
        group.MapPost("/{codigoPlanta}/{codigo}/devolver", async (
            string codigoPlanta,
            string codigo,
            DevolverValeRequest? body,
            ISender sender) =>
            Results.Ok(await sender.Send(new DevolverValeCommand(
                codigoPlanta,
                codigo,
                body?.Articulos))))
        .WithName("DevolverVale")
        .RequireAuthorization("VALESSALIDA.UPDATE");

        // Anulación: mismo permiso UPDATE que las demás transacciones de Logística.
        group.MapDelete("/{codigoPlanta}/{codigo}", async (
            string codigoPlanta,
            string codigo,
            ISender sender) =>
            Results.Ok(await sender.Send(new AnulateValeCommand(codigoPlanta, codigo))))
        .WithName("AnulateVale")
        .RequireAuthorization("VALESSALIDA.UPDATE");
    }

    public record ChangeEstadoBody(string NuevoEstado);
}
