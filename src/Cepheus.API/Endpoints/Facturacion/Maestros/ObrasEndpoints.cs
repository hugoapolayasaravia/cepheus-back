using Cepheus.Application.Features.Facturacion.Maestros.Obras.ChangeObraEstado;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.CreateObra;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.GetObraByCode;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.GetObrasPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.UpdateObra;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class ObrasEndpoints
    {
        public static void MapObrasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/maestros/obras")
                .WithTags("Obras (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateObraCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/maestros/obras/{result.ClienteCode}/{result.Code}", result);
            })
            .WithName("CreateObra")
            .RequireAuthorization("OBRAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetObrasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetObrasPagedQueryString")
            .RequireAuthorization("OBRAS.VIEW");

            group.MapPost("/paged/body", async (
                GetObrasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetObrasPagedBody")
            .RequireAuthorization("OBRAS.VIEW");

            group.MapGet("/{codigoCliente}/{codigo}", async (string codigoCliente, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetObraByCodeQuery(codigoCliente, codigo));
                return Results.Ok(result);
            })
            .WithName("GetObraByCode")
            .RequireAuthorization("OBRAS.VIEW");

            group.MapPut("/{codigoCliente}/{codigo}", async (
                string codigoCliente, string codigo, UpdateObraCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoCliente, command.ClienteCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateObra")
            .RequireAuthorization("OBRAS.UPDATE");

            group.MapPatch("/{codigoCliente}/{codigo}/estado", async (
                string codigoCliente, string codigo, ChangeEstadoBody body, ISender sender) =>
            {
                var result = await sender.Send(new ChangeObraEstadoCommand(codigoCliente, codigo, body.NuevoEstado));
                return Results.Ok(result);
            })
            .WithName("ChangeObraEstado")
            .RequireAuthorization("OBRAS.UPDATE");
        }

        public record ChangeEstadoBody(string NuevoEstado);
    }
}
