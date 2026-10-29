using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.CreateTipoCliente;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.GetTipoClienteByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.GetTiposClientePaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.ToggleTipoClienteStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.UpdateTipoCliente;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class TiposClienteEndpoints
    {
        public static void MapTiposClienteEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/tipos-cliente")
                .WithTags("Tipos de Cliente (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoClienteCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/tipos-cliente/{result.Code}", result);
            })
            .WithName("CreateTipoCliente")
            .RequireAuthorization("TIPOSCLIENTE.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposClientePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposClientePagedQueryString")
            .RequireAuthorization("TIPOSCLIENTE.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposClientePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposClientePagedBody")
            .RequireAuthorization("TIPOSCLIENTE.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoClienteByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoClienteByCode")
            .RequireAuthorization("TIPOSCLIENTE.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoClienteCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoCliente")
            .RequireAuthorization("TIPOSCLIENTE.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoClienteStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoClienteStatus")
            .RequireAuthorization("TIPOSCLIENTE.UPDATE");
        }
    }
}
