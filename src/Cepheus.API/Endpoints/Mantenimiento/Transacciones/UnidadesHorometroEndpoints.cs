using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.CreateUnidadHorometro;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.DeleteUnidadHorometro;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.GetUnidadHorometroByKey;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.GetUnidadesHorometroPaginated;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.UpdateUnidadHorometro;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Transacciones
{
    public static class UnidadesHorometroEndpoints
    {
        public static void MapUnidadesHorometroEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/transacciones/unidades-horometro")
                .WithTags("Horómetro de unidad (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateUnidadHorometroCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/mantenimiento/transacciones/unidades-horometro/{result.SubCentroCostoCode}/{result.Fecha:yyyy-MM-dd}",
                    result);
            })
            .WithName("CreateUnidadHorometro")
            .RequireAuthorization("UNIDADESHOROMETRO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetUnidadesHorometroPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUnidadesHorometroPagedQueryString")
            .RequireAuthorization("UNIDADESHOROMETRO.VIEW");

            group.MapPost("/paged/body", async (
                GetUnidadesHorometroPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUnidadesHorometroPagedBody")
            .RequireAuthorization("UNIDADESHOROMETRO.VIEW");

            group.MapGet("/{subCentroCosto}/{fecha:datetime}", async (
                string subCentroCosto, DateTime fecha, ISender sender) =>
            {
                var result = await sender.Send(new GetUnidadHorometroByKeyQuery(subCentroCosto, fecha));
                return Results.Ok(result);
            })
            .WithName("GetUnidadHorometroByKey")
            .RequireAuthorization("UNIDADESHOROMETRO.VIEW");

            group.MapPut("/{subCentroCosto}/{fecha:datetime}", async (
                string subCentroCosto, DateTime fecha,
                UpdateUnidadHorometroCommand command, ISender sender) =>
            {
                if (!string.Equals(subCentroCosto, command.SubCentroCostoCode, StringComparison.OrdinalIgnoreCase) ||
                    fecha.Date != command.Fecha.Date)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateUnidadHorometro")
            .RequireAuthorization("UNIDADESHOROMETRO.UPDATE");

            group.MapDelete("/{subCentroCosto}/{fecha:datetime}", async (
                string subCentroCosto, DateTime fecha, ISender sender) =>
            {
                await sender.Send(new DeleteUnidadHorometroCommand(subCentroCosto, fecha));
                return Results.NoContent();
            })
            .WithName("DeleteUnidadHorometro")
            .RequireAuthorization("UNIDADESHOROMETRO.DELETE");
        }
    }
}
