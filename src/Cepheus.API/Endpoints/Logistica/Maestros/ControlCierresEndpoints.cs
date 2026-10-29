using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.CreateControlCierre;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.GetControlCierre;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.GetControlCierresByPlanta;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.UpdateControlCierre;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ControlCierresEndpoints
    {
        public static void MapControlCierresEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/control-cierres")
                .WithTags("Control de Cierres (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateControlCierreCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/maestros/control-cierres/{result.PlantaCode}/{result.PeriodCode}", result);
            })
            .WithName("CreateControlCierre")
            .RequireAuthorization("CONTROLCIERRES.CREATE");

            group.MapGet("/planta/{codigoPlanta}", async (string codigoPlanta, ISender sender) =>
            {
                var result = await sender.Send(new GetControlCierresByPlantaQuery(codigoPlanta));
                return Results.Ok(result);
            })
            .WithName("GetControlCierresByPlanta")
            .RequireAuthorization("CONTROLCIERRES.VIEW");

            group.MapGet("/{codigoPlanta}/{periodo}", async (string codigoPlanta, string periodo, ISender sender) =>
            {
                var result = await sender.Send(new GetControlCierreQuery(codigoPlanta, periodo));
                return Results.Ok(result);
            })
            .WithName("GetControlCierre")
            .RequireAuthorization("CONTROLCIERRES.VIEW");

            group.MapPut("/{codigoPlanta}/{periodo}", async (
                string codigoPlanta, string periodo, UpdateControlCierreCommand command, ISender sender) =>
            {
                if (codigoPlanta != command.PlantaCode || periodo != command.PeriodCode)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateControlCierre")
            .RequireAuthorization("CONTROLCIERRES.UPDATE");
        }
    }
}
