using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.ChangeOrdenTrabajoEstado;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.CreateOrdenTrabajo;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.GetOrdenesTrabajoPaginated;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.GetOrdenTrabajoByCode;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.UpdateOrdenTrabajo;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Transacciones
{
    public static class OrdenesTrabajoEndpoints
    {
        public static void MapOrdenesTrabajoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/transacciones/ordenes-trabajo")
                .WithTags("Órdenes de Trabajo (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateOrdenTrabajoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/mantenimiento/transacciones/ordenes-trabajo/{result.PlantaCode}/{result.Code}", result);
            })
            .WithName("CreateOrdenTrabajo")
            .RequireAuthorization("ORDENESTRABAJO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetOrdenesTrabajoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetOrdenesTrabajoPagedQueryString")
            .RequireAuthorization("ORDENESTRABAJO.VIEW");

            group.MapPost("/paged/body", async (
                GetOrdenesTrabajoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetOrdenesTrabajoPagedBody")
            .RequireAuthorization("ORDENESTRABAJO.VIEW");

            group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetOrdenTrabajoByCodeQuery(codigoPlanta, codigo));
                return Results.Ok(result);
            })
            .WithName("GetOrdenTrabajoByCode")
            .RequireAuthorization("ORDENESTRABAJO.VIEW");

            group.MapPut("/{codigoPlanta}/{codigo}", async (
                string codigoPlanta, string codigo, UpdateOrdenTrabajoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateOrdenTrabajo")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");

            // No hay toggle-status: el ciclo de vida tiene 5 estados con
            // transiciones válidas propias (ver ChangeOrdenTrabajoEstadoCommandHandler),
            // no un simple activo/inactivo.
            group.MapPatch("/{codigoPlanta}/{codigo}/estado", async (
                string codigoPlanta, string codigo, ChangeEstadoBody body, ISender sender) =>
            {
                var result = await sender.Send(new ChangeOrdenTrabajoEstadoCommand(codigoPlanta, codigo, body.NuevoEstado));
                return Results.Ok(result);
            })
            .WithName("ChangeOrdenTrabajoEstado")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");
        }

        public record ChangeEstadoBody(string NuevoEstado);
    }
}
