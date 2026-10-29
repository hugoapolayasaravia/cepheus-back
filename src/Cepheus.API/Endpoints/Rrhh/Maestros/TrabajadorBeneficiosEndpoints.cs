using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.CreateTrabajadorBeneficio;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.DeleteTrabajadorBeneficio;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.GetTrabajadorBeneficiosByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.UpdateTrabajadorBeneficio;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorBeneficiosEndpoints
    {
        public static void MapTrabajadorBeneficiosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-beneficios")
                .WithTags("TrabajadorBeneficio (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorBeneficioCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-beneficios/trabajador/{trabajadorCode}", result);
            })
            .WithName("CreateTrabajadorBeneficio")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorBeneficiosByTrabajadorQuery(trabajadorCode));
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .WithName("GetTrabajadorBeneficioByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorBeneficioCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorBeneficio")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorBeneficioCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorBeneficio")
            .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}
