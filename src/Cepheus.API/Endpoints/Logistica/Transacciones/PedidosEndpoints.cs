// Cepheus.API/Endpoints/Logistica/Transacciones/PedidosEndpoints.cs
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.ChangeEstadoPedido;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetPedidoByCode;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetPedidosPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.CreatePedidoDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.DeletePedidoDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.UpdatePedidoDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.UpdatePedido;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones
{
    public static class PedidosEndpoints
    {
        public static void MapPedidosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/transacciones/pedidos")
                .WithTags("Pedidos (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreatePedidoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/transacciones/pedidos/{result.PlantaCode}/{result.Code}", result);
            })
            .WithName("CreatePedido")
            .RequireAuthorization("PEDIDOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetPedidosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPedidosPagedQueryString")
            .RequireAuthorization("PEDIDOS.VIEW");

            group.MapPost("/paged/body", async (
                GetPedidosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPedidosPagedBody")
            .RequireAuthorization("PEDIDOS.VIEW");

            group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetPedidoByCodeQuery(codigoPlanta, codigo));
                return Results.Ok(result);
            })
            .WithName("GetPedidoByCode")
            .RequireAuthorization("PEDIDOS.VIEW");

            group.MapPut("/{codigoPlanta}/{codigo}", async (
                string codigoPlanta, string codigo, UpdatePedidoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdatePedido")
            .RequireAuthorization("PEDIDOS.UPDATE");

            // Sin toggle-status: el ciclo de vida tiene 9 estados con
            // transiciones propias (ver ChangeEstadoPedidoCommandHandler).
            group.MapPatch("/{codigoPlanta}/{codigo}/estado", async (
                string codigoPlanta, string codigo, ChangeEstadoBody body, ISender sender) =>
            {
                var result = await sender.Send(new ChangeEstadoPedidoCommand(codigoPlanta, codigo, body.NuevoEstado));
                return Results.Ok(result);
            })
            .WithName("ChangeEstadoPedido")
            .RequireAuthorization("PEDIDOS.UPDATE");

            // Líneas de detalle: reusan los permisos de Pedidos (mismo
            // criterio que ProveedorDirecciones/Trabajador* en RRHH).
            group.MapPost("/{codigoPlanta}/{codigoPedido}/detalles", async (
                string codigoPlanta, string codigoPedido, CreatePedidoDetalleLineaBody body, ISender sender) =>
            {
                var command = new CreatePedidoDetalleCommand(
                    codigoPlanta, codigoPedido, body.ArticuloCode, body.DescripcionArticulo,
                    body.UnidadMedidaCode, body.PrecioArticulo, body.CantidadArticulo, body.ProveedorCode);
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/transacciones/pedidos/{codigoPlanta}/{codigoPedido}", result);
            })
            .WithName("CreatePedidoDetalle")
            .RequireAuthorization("PEDIDOS.UPDATE");

            group.MapPut("/{codigoPlanta}/{codigoPedido}/detalles/{item:int}", async (
                string codigoPlanta, string codigoPedido, int item, UpdatePedidoDetalleCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoPedido, command.PedidoCode, StringComparison.OrdinalIgnoreCase) ||
                    item != command.ItemNumber)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdatePedidoDetalle")
            .RequireAuthorization("PEDIDOS.UPDATE");

            group.MapDelete("/{codigoPlanta}/{codigoPedido}/detalles/{item:int}", async (
                string codigoPlanta, string codigoPedido, int item, ISender sender) =>
            {
                var result = await sender.Send(new DeletePedidoDetalleCommand(codigoPlanta, codigoPedido, item));
                return Results.Ok(result);
            })
            .WithName("DeletePedidoDetalle")
            .RequireAuthorization("PEDIDOS.UPDATE");
        }

        public record ChangeEstadoBody(string NuevoEstado);

        public record CreatePedidoDetalleLineaBody(
            string? ArticuloCode,
            string DescripcionArticulo,
            string UnidadMedidaCode,
            decimal PrecioArticulo,
            decimal CantidadArticulo,
            string? ProveedorCode);
    }
}