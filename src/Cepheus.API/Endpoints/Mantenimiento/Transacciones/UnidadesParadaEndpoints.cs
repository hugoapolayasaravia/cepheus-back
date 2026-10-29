using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.CreateUnidadParada;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.DeleteUnidadParada;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.GetUnidadParadaByKey;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.GetUnidadesParadaPaginated;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.UpdateUnidadParada;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Transacciones
{
    public static class UnidadesParadaEndpoints
    {
        public static void MapUnidadesParadaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/transacciones/unidades-parada")
                .WithTags("Paradas de unidad (Mantenimiento)")
                .RequireAuthorization();

            // Agregar o actualizar por (subcentro, fecha). 204 si Horas = 0 (no se registra nada).
            group.MapPost("/", async (CreateUnidadParadaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return result is null ? Results.NoContent() : Results.Ok(result);
            })
            .WithName("CreateUnidadParada")
            .RequireAuthorization("UNIDADESPARADA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetUnidadesParadaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUnidadesParadaPagedQueryString")
            .RequireAuthorization("UNIDADESPARADA.VIEW");

            group.MapPost("/paged/body", async (
                GetUnidadesParadaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUnidadesParadaPagedBody")
            .RequireAuthorization("UNIDADESPARADA.VIEW");

            group.MapGet("/{subCentroCosto}/{fecha:datetime}", async (
                string subCentroCosto, DateTime fecha, ISender sender) =>
            {
                var result = await sender.Send(new GetUnidadParadaByKeyQuery(subCentroCosto, fecha));
                return Results.Ok(result);
            })
            .WithName("GetUnidadParadaByKey")
            .RequireAuthorization("UNIDADESPARADA.VIEW");

            group.MapPut("/{subCentroCosto}/{fecha:datetime}", async (
                string subCentroCosto, DateTime fecha,
                UpdateUnidadParadaCommand command, ISender sender) =>
            {
                if (!string.Equals(subCentroCosto, command.SubCentroCostoCode, StringComparison.OrdinalIgnoreCase) ||
                    fecha.Date != command.Fecha.Date)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateUnidadParada")
            .RequireAuthorization("UNIDADESPARADA.UPDATE");

            group.MapDelete("/{subCentroCosto}/{fecha:datetime}", async (
                string subCentroCosto, DateTime fecha, ISender sender) =>
            {
                await sender.Send(new DeleteUnidadParadaCommand(subCentroCosto, fecha));
                return Results.NoContent();
            })
            .WithName("DeleteUnidadParada")
            .RequireAuthorization("UNIDADESPARADA.DELETE");
        }
    }
}
