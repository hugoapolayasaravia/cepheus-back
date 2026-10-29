using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AjusteInventarioDetalles.CreateAjusteInventarioDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AjusteInventarioDetalles.DeleteAjusteInventarioDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AjusteInventarioDetalles.UpdateAjusteInventarioDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AnulateAjusteInventario;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.CreateAjusteInventario;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.DevolverAjusteInventario;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.GetAjusteInventarioArticuloInfo;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.GetAjusteInventarioByCode;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.GetAjustesInventarioPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.ProcesarAjusteInventario;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.UpdateAjusteInventario;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones;

public static class AjustesInventarioEndpoints
{
    public static void MapAjustesInventarioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/transacciones/ajustes-inventario")
            .WithTags("Ajustes de Inventario (Logística)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateAjusteInventarioCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            return Results.Created(
                $"/api/logistica/transacciones/ajustes-inventario/{result.PlantaCode}/{result.Code}",
                result);
        })
        .WithName("CreateAjusteInventario")
        .RequireAuthorization("AJUSTESINVENTARIO.CREATE");

        // Listado: parámetros literales de Logi_sp_Listado_MAjustes.
        group.MapGet("/paged", async ([AsParameters] GetAjustesInventarioPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetAjustesInventarioPagedQueryString")
        .RequireAuthorization("AJUSTESINVENTARIO.VIEW");

        group.MapPost("/paged/body", async (GetAjustesInventarioPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetAjustesInventarioPagedBody")
        .RequireAuthorization("AJUSTESINVENTARIO.VIEW");

        // Stock, reservado y precio promedio de un artículo para armar una línea.
        group.MapGet("/articulos/{plantaCode}/{articuloCode}", async (
            string plantaCode,
            string articuloCode,
            ISender sender) =>
            Results.Ok(await sender.Send(new GetAjusteInventarioArticuloInfoQuery(plantaCode, articuloCode))))
        .WithName("GetAjusteInventarioArticuloInfo")
        .RequireAuthorization("AJUSTESINVENTARIO.VIEW");

        group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
            Results.Ok(await sender.Send(new GetAjusteInventarioByCodeQuery(codigoPlanta, codigo))))
        .WithName("GetAjusteInventarioByCode")
        .RequireAuthorization("AJUSTESINVENTARIO.VIEW");

        group.MapPut("/{codigoPlanta}/{codigo}", async (
            string codigoPlanta,
            string codigo,
            UpdateAjusteInventarioRequest body,
            ISender sender) =>
            Results.Ok(await sender.Send(new UpdateAjusteInventarioCommand(
                codigoPlanta,
                codigo,
                body.FechaEntrega,
                body.Observacion))))
        .WithName("UpdateAjusteInventario")
        .RequireAuthorization("AJUSTESINVENTARIO.UPDATE");

        // ---------------------------------------------------------------- líneas

        group.MapPost("/{codigoPlanta}/{codigo}/detalles", async (
            string codigoPlanta,
            string codigo,
            CreateAjusteInventarioDetalleRequest body,
            ISender sender) =>
            Results.Ok(await sender.Send(new CreateAjusteInventarioDetalleCommand(
                codigoPlanta,
                codigo,
                body.ArticuloCode,
                body.Tipo,
                body.Cantidad))))
        .WithName("CreateAjusteInventarioDetalle")
        .RequireAuthorization("AJUSTESINVENTARIO.UPDATE");

        group.MapPut("/{codigoPlanta}/{codigo}/detalles/{articuloCode}", async (
            string codigoPlanta,
            string codigo,
            string articuloCode,
            UpdateAjusteInventarioDetalleRequest body,
            ISender sender) =>
            Results.Ok(await sender.Send(new UpdateAjusteInventarioDetalleCommand(
                codigoPlanta,
                codigo,
                articuloCode,
                body.Tipo,
                body.Cantidad))))
        .WithName("UpdateAjusteInventarioDetalle")
        .RequireAuthorization("AJUSTESINVENTARIO.UPDATE");

        group.MapDelete("/{codigoPlanta}/{codigo}/detalles/{articuloCode}", async (
            string codigoPlanta,
            string codigo,
            string articuloCode,
            ISender sender) =>
            Results.Ok(await sender.Send(new DeleteAjusteInventarioDetalleCommand(
                codigoPlanta,
                codigo,
                articuloCode))))
        .WithName("DeleteAjusteInventarioDetalle")
        .RequireAuthorization("AJUSTESINVENTARIO.UPDATE");

        // ---------------------------------------------------------------- flujo

        // Articulos vacío = todas las líneas pendientes; con lista = proceso por ítem.
        group.MapPost("/{codigoPlanta}/{codigo}/procesar", async (
            string codigoPlanta,
            string codigo,
            ProcesarAjusteInventarioRequest? body,
            ISender sender) =>
            Results.Ok(await sender.Send(new ProcesarAjusteInventarioCommand(
                codigoPlanta,
                codigo,
                body?.Articulos))))
        .WithName("ProcesarAjusteInventario")
        .RequireAuthorization("AJUSTESINVENTARIO.UPDATE");

        // Articulos vacío = todas las líneas procesadas; con lista = devolución por ítem.
        group.MapPost("/{codigoPlanta}/{codigo}/devolver", async (
            string codigoPlanta,
            string codigo,
            DevolverAjusteInventarioRequest? body,
            ISender sender) =>
            Results.Ok(await sender.Send(new DevolverAjusteInventarioCommand(
                codigoPlanta,
                codigo,
                body?.Articulos))))
        .WithName("DevolverAjusteInventario")
        .RequireAuthorization("AJUSTESINVENTARIO.UPDATE");

        // Anulación: mismo permiso UPDATE que las demás transacciones de Logística.
        group.MapDelete("/{codigoPlanta}/{codigo}", async (
            string codigoPlanta,
            string codigo,
            ISender sender) =>
            Results.Ok(await sender.Send(new AnulateAjusteInventarioCommand(codigoPlanta, codigo))))
        .WithName("AnulateAjusteInventario")
        .RequireAuthorization("AJUSTESINVENTARIO.UPDATE");
    }
}
