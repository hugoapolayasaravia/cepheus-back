using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.CreateUnidadMedidaVenta;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.GetUnidadMedidaVentaById;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.GetUnidadesMedidaVentaPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.ToggleUnidadMedidaVentaStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.UpdateUnidadMedidaVenta;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class UnidadesMedidaVentaEndpoints
    {
        public static void MapUnidadesMedidaVentaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/unidades-medida")
                .WithTags("Unidades de medida (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateUnidadMedidaVentaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/unidades-medida/{result.Code}", result);
            })
            .WithName("CreateUnidadMedidaVenta")
            .RequireAuthorization("UNIDADESMEDIDAVENTA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetUnidadesMedidaVentaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUnidadesMedidaVentaPagedQueryString")
            .RequireAuthorization("UNIDADESMEDIDAVENTA.VIEW");

            group.MapPost("/paged/body", async (
                GetUnidadesMedidaVentaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUnidadesMedidaVentaPagedBody")
            .RequireAuthorization("UNIDADESMEDIDAVENTA.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetUnidadMedidaVentaByIdQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetUnidadMedidaVentaById")
            .RequireAuthorization("UNIDADESMEDIDAVENTA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateUnidadMedidaVentaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateUnidadMedidaVenta")
            .RequireAuthorization("UNIDADESMEDIDAVENTA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleUnidadMedidaVentaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleUnidadMedidaVentaStatus")
            .RequireAuthorization("UNIDADESMEDIDAVENTA.UPDATE");
        }
    }
}
