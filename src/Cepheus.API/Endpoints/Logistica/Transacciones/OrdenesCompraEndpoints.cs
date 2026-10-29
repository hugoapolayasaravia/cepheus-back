// Cepheus.API/Endpoints/Logistica/Transacciones/OrdenesCompraEndpoints.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenCompraDetalles.CreateOrdenCompraDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenCompraDetalles.DeleteOrdenCompraDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.ChangeEstadoOrdenCompra;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.CopyOrdenCompra;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.CreateOrdenCompra;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.GetOrdenCompraByCode;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.GetOrdenesCompraPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.UpdateOrdenCompra;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones
{
    public static class OrdenesCompraEndpoints
    {
        public static void MapOrdenesCompraEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/transacciones/ordenes-compra")
                .WithTags("Órdenes de Compra (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateOrdenCompraCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/transacciones/ordenes-compra/{result.PlantaCode}/{result.Code}", result);
            })
            .WithName("CreateOrdenCompra")
            .RequireAuthorization("ORDENESCOMPRA.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetOrdenesCompraPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetOrdenesCompraPagedQueryString")
            .RequireAuthorization("ORDENESCOMPRA.VIEW");

            group.MapPost("/paged/body", async (GetOrdenesCompraPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetOrdenesCompraPagedBody")
            .RequireAuthorization("ORDENESCOMPRA.VIEW");

            group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new GetOrdenCompraByCodeQuery(codigoPlanta, codigo))))
            .WithName("GetOrdenCompraByCode")
            .RequireAuthorization("ORDENESCOMPRA.VIEW");

            group.MapPut("/{codigoPlanta}/{codigo}", async (
                string codigoPlanta, string codigo, UpdateOrdenCompraCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateOrdenCompra")
            .RequireAuthorization("ORDENESCOMPRA.UPDATE");

            group.MapPatch("/{codigoPlanta}/{codigo}/estado", async (
                string codigoPlanta, string codigo, ChangeEstadoBody body, ISender sender) =>
                Results.Ok(await sender.Send(new ChangeEstadoOrdenCompraCommand(codigoPlanta, codigo, body.NuevoEstado))))
            .WithName("ChangeEstadoOrdenCompra")
            .RequireAuthorization("ORDENESCOMPRA.UPDATE");

            group.MapPost("/{codigoPlanta}/{codigo}/copiar", async (
                string codigoPlanta, string codigo, CopyOrdenCompraBody body, ISender sender) =>
            {
                var result = await sender.Send(new CopyOrdenCompraCommand(codigoPlanta, codigo, body.NuevaFechaEntrega));
                return Results.Created($"/api/logistica/transacciones/ordenes-compra/{result.PlantaCode}/{result.Code}", result);
            })
            .WithName("CopyOrdenCompra")
            .RequireAuthorization("ORDENESCOMPRA.CREATE");

            group.MapPost("/{codigoPlanta}/{codigoOrden}/detalles", async (
                string codigoPlanta, string codigoOrden, CreateOrdenCompraDetalleCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoOrden, command.OrdenCompraCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("CreateOrdenCompraDetalle")
            .RequireAuthorization("ORDENESCOMPRA.UPDATE");

            group.MapDelete("/{codigoPlanta}/{codigoOrden}/detalles/{codigoArticulo}", async (
                string codigoPlanta, string codigoOrden, string codigoArticulo, ISender sender) =>
                Results.Ok(await sender.Send(new DeleteOrdenCompraDetalleCommand(codigoPlanta, codigoOrden, codigoArticulo))))
            .WithName("DeleteOrdenCompraDetalle")
            .RequireAuthorization("ORDENESCOMPRA.UPDATE");
        }

        public record ChangeEstadoBody(string NuevoEstado);
        public record CopyOrdenCompraBody(DateTime NuevaFechaEntrega);
    }
}