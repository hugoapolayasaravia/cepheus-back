using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.CreateSegmentoVentas;
using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.GetSegmentoVentasByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.GetSegmentosVentasPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.ToggleSegmentoVentasStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.UpdateSegmentoVentas;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class SegmentosVentasEndpoints
    {
        public static void MapSegmentosVentasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/segmentos-ventas")
                .WithTags("Segmentos de Ventas (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSegmentoVentasCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/segmentos-ventas/{result.Code}", result);
            })
            .WithName("CreateSegmentoVentas")
            .RequireAuthorization("SEGMENTOSVENTAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSegmentosVentasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSegmentosVentasPagedQueryString")
            .RequireAuthorization("SEGMENTOSVENTAS.VIEW");

            group.MapPost("/paged/body", async (
                GetSegmentosVentasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSegmentosVentasPagedBody")
            .RequireAuthorization("SEGMENTOSVENTAS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetSegmentoVentasByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetSegmentoVentasByCode")
            .RequireAuthorization("SEGMENTOSVENTAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateSegmentoVentasCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSegmentoVentas")
            .RequireAuthorization("SEGMENTOSVENTAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleSegmentoVentasStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleSegmentoVentasStatus")
            .RequireAuthorization("SEGMENTOSVENTAS.UPDATE");
        }
    }
}
