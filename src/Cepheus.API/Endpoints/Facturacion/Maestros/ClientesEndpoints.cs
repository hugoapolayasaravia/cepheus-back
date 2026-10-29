using Cepheus.Application.Features.Facturacion.Maestros.Clientes.ChangeClienteEstado;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.CreateCliente;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.GetClienteByCode;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.GetClientesPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.UpdateCliente;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class ClientesEndpoints
    {
        public static void MapClientesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/maestros/clientes")
                .WithTags("Clientes (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateClienteCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/maestros/clientes/{result.Code}", result);
            })
            .WithName("CreateCliente")
            .RequireAuthorization("CLIENTES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetClientesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetClientesPagedQueryString")
            .RequireAuthorization("CLIENTES.VIEW");

            group.MapPost("/paged/body", async (
                GetClientesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetClientesPagedBody")
            .RequireAuthorization("CLIENTES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetClienteByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetClienteByCode")
            .RequireAuthorization("CLIENTES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateClienteCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCliente")
            .RequireAuthorization("CLIENTES.UPDATE");

            group.MapPatch("/{codigo}/estado", async (string codigo, ChangeClienteEstadoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("ChangeClienteEstado")
            .RequireAuthorization("CLIENTES.UPDATE");
        }
    }
}
