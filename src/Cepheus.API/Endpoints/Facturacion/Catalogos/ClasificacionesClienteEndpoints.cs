using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.CreateClasificacionCliente;
using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.GetClasificacionClienteByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.GetClasificacionesClientePaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.ToggleClasificacionClienteStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.UpdateClasificacionCliente;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class ClasificacionesClienteEndpoints
    {
        public static void MapClasificacionesClienteEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/clasificaciones-cliente")
                .WithTags("Clasificaciones de Cliente (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateClasificacionClienteCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/clasificaciones-cliente/{result.Code}", result);
            })
            .WithName("CreateClasificacionCliente")
            .RequireAuthorization("CLASIFICACIONCLIENTE.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetClasificacionesClientePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetClasificacionesClientePagedQueryString")
            .RequireAuthorization("CLASIFICACIONCLIENTE.VIEW");

            group.MapPost("/paged/body", async (
                GetClasificacionesClientePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetClasificacionesClientePagedBody")
            .RequireAuthorization("CLASIFICACIONCLIENTE.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetClasificacionClienteByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetClasificacionClienteByCode")
            .RequireAuthorization("CLASIFICACIONCLIENTE.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateClasificacionClienteCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateClasificacionCliente")
            .RequireAuthorization("CLASIFICACIONCLIENTE.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleClasificacionClienteStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleClasificacionClienteStatus")
            .RequireAuthorization("CLASIFICACIONCLIENTE.UPDATE");
        }
    }
}
