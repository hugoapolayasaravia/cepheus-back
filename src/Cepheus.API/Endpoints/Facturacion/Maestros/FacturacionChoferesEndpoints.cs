using Cepheus.Application.Features.Facturacion.Maestros.Choferes.CreateChofer;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.GetChoferByCode;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.GetChoferesPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.ToggleChoferStatus;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.UpdateChofer;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class FacturacionChoferesEndpoints
    {
        public static void MapFacturacionChoferesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/maestros/choferes")
                .WithTags("Choferes (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateChoferCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/maestros/choferes/{result.TransportistaCode}/{result.Code}", result);
            })
            .WithName("CreateFacturacionChofer")
            .RequireAuthorization("FAC_CHOFERES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetChoferesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFacturacionChoferesPagedQueryString")
            .RequireAuthorization("FAC_CHOFERES.VIEW");

            group.MapPost("/paged/body", async (
                GetChoferesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFacturacionChoferesPagedBody")
            .RequireAuthorization("FAC_CHOFERES.VIEW");

            group.MapGet("/{codigoTransportista}/{codigo}", async (
                string codigoTransportista, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetChoferByCodeQuery(codigoTransportista, codigo));
                return Results.Ok(result);
            })
            .WithName("GetFacturacionChoferByCode")
            .RequireAuthorization("FAC_CHOFERES.VIEW");

            group.MapPut("/{codigoTransportista}/{codigo}", async (
                string codigoTransportista, string codigo, UpdateChoferCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoTransportista, command.TransportistaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateFacturacionChofer")
            .RequireAuthorization("FAC_CHOFERES.UPDATE");

            group.MapPatch("/{codigoTransportista}/{codigo}/toggle-status", async (
                string codigoTransportista, string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleChoferStatusCommand(codigoTransportista, codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleFacturacionChoferStatus")
            .RequireAuthorization("FAC_CHOFERES.UPDATE");
        }
    }
}
