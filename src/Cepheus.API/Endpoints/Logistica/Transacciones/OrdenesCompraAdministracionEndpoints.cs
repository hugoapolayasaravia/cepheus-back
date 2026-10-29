// Cepheus.API/Endpoints/Logistica/Transacciones/OrdenesCompraAdministracionEndpoints.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.ChangeEstadoOrdenCompraAdm;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.CreateOrdenCompraAdm;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.GetOrdenCompraAdmByCode;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.GetOrdenesCompraAdmPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.CreateOrdenCompraAdmDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.DeleteOrdenCompraAdmDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.UpdateOrdenCompraAdmDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.UpdateOrdenCompraAdm;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones
{
    public static class OrdenesCompraAdministracionEndpoints
    {
        public static void MapOrdenesCompraAdministracionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/transacciones/ordenes-compra-administracion")
                .WithTags("Órdenes de Compra de Administración (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateOrdenCompraAdmCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/transacciones/ordenes-compra-administracion/{result.Code}", result);
            })
            .WithName("CreateOrdenCompraAdministracion")
            .RequireAuthorization("ORDENESCOMPRAADM.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetOrdenesCompraAdmPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetOrdenesCompraAdministracionPagedQueryString")
            .RequireAuthorization("ORDENESCOMPRAADM.VIEW");

            group.MapPost("/paged/body", async (GetOrdenesCompraAdmPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetOrdenesCompraAdministracionPagedBody")
            .RequireAuthorization("ORDENESCOMPRAADM.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new GetOrdenCompraAdmByCodeQuery(codigo))))
            .WithName("GetOrdenCompraAdministracionByCode")
            .RequireAuthorization("ORDENESCOMPRAADM.VIEW");

            group.MapPut("/{codigo}", async (
                string codigo, UpdateOrdenCompraAdmCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateOrdenCompraAdministracion")
            .RequireAuthorization("ORDENESCOMPRAADM.UPDATE");

            group.MapPatch("/{codigo}/estado", async (
                string codigo, ChangeEstadoBody body, ISender sender) =>
                Results.Ok(await sender.Send(new ChangeEstadoOrdenCompraAdmCommand(codigo, body.NuevoEstado))))
            .WithName("ChangeEstadoOrdenCompraAdministracion")
            .RequireAuthorization("ORDENESCOMPRAADM.UPDATE");

            group.MapPost("/{codigo}/detalles", async (
                string codigo, CreateOrdenCompraAdmDetalleCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.OrdenCompraAdministracionCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("CreateOrdenCompraAdministracionDetalle")
            .RequireAuthorization("ORDENESCOMPRAADM.UPDATE");

            group.MapPut("/{codigo}/detalles/{item:int}", async (
                string codigo, int item, UpdateOrdenCompraAdmDetalleCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.OrdenCompraAdministracionCode, StringComparison.OrdinalIgnoreCase)
                    || item != command.ItemNumber)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateOrdenCompraAdministracionDetalle")
            .RequireAuthorization("ORDENESCOMPRAADM.UPDATE");

            group.MapDelete("/{codigo}/detalles/{item:int}", async (
                string codigo, int item, ISender sender) =>
                Results.Ok(await sender.Send(new DeleteOrdenCompraAdmDetalleCommand(codigo, item))))
            .WithName("DeleteOrdenCompraAdministracionDetalle")
            .RequireAuthorization("ORDENESCOMPRAADM.UPDATE");
        }

        public record ChangeEstadoBody(string NuevoEstado);
    }
}
