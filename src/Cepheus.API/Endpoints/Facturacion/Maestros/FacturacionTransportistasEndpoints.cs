using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.CreateTransportista;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.GetTransportistaByCode;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.GetTransportistasPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.ToggleTransportistaStatus;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.UpdateTransportista;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class FacturacionTransportistasEndpoints
    {
        public static void MapFacturacionTransportistasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/maestros/transportistas")
                .WithTags("Transportistas (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTransportistaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/maestros/transportistas/{result.Code}", result);
            })
            .WithName("CreateFacturacionTransportista")
            .RequireAuthorization("FAC_TRANSPORTISTAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTransportistasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFacturacionTransportistasPagedQueryString")
            .RequireAuthorization("FAC_TRANSPORTISTAS.VIEW");

            group.MapPost("/paged/body", async (
                GetTransportistasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFacturacionTransportistasPagedBody")
            .RequireAuthorization("FAC_TRANSPORTISTAS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTransportistaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetFacturacionTransportistaByCode")
            .RequireAuthorization("FAC_TRANSPORTISTAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTransportistaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateFacturacionTransportista")
            .RequireAuthorization("FAC_TRANSPORTISTAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTransportistaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleFacturacionTransportistaStatus")
            .RequireAuthorization("FAC_TRANSPORTISTAS.UPDATE");
        }
    }
}
