using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.CreateVendedor;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.GetVendedorByCode;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.GetVendedoresPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.ToggleVendedorStatus;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.UpdateVendedor;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class VendedoresEndpoints
    {
        public static void MapVendedoresEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/maestros/vendedores")
                .WithTags("Vendedores")
                .RequireAuthorization();

            group.MapPost("/", async (CreateVendedorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/maestros/vendedores/{result.Code}", result);
            })
            .WithName("CreateVendedor")
            .RequireAuthorization("VENDEDORES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetVendedoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetVendedoresPagedQueryString")
            .RequireAuthorization("VENDEDORES.VIEW");

            group.MapPost("/paged/body", async (
                GetVendedoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetVendedoresPagedBody")
            .RequireAuthorization("VENDEDORES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetVendedorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetVendedorByCode")
            .RequireAuthorization("VENDEDORES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateVendedorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateVendedor")
            .RequireAuthorization("VENDEDORES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleVendedorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleVendedorStatus")
            .RequireAuthorization("VENDEDORES.UPDATE");
        }
    }
}
