using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.CreateAnalisisVenta;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.GetAnalisisVentaByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.GetAnalisisVentasPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.ToggleAnalisisVentaStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.UpdateAnalisisVenta;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class AnalisisVentasEndpoints
    {
        public static void MapAnalisisVentasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/analisis-ventas")
                .WithTags("Análisis de Ventas (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateAnalisisVentaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/analisis-ventas/{result.Code}", result);
            })
            .WithName("CreateAnalisisVenta")
            .RequireAuthorization("ANALISISVENTAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetAnalisisVentasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAnalisisVentasPagedQueryString")
            .RequireAuthorization("ANALISISVENTAS.VIEW");

            group.MapPost("/paged/body", async (
                GetAnalisisVentasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAnalisisVentasPagedBody")
            .RequireAuthorization("ANALISISVENTAS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetAnalisisVentaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetAnalisisVentaByCode")
            .RequireAuthorization("ANALISISVENTAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateAnalisisVentaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateAnalisisVenta")
            .RequireAuthorization("ANALISISVENTAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleAnalisisVentaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleAnalisisVentaStatus")
            .RequireAuthorization("ANALISISVENTAS.UPDATE");
        }
    }
}
