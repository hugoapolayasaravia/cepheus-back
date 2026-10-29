using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.CopyPlanPreventivo;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.CreatePlanPreventivo;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.DeletePlanPreventivo;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.GetPlanPreventivoByKey;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.GetPlanesPreventivoPaginated;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.UpdatePlanPreventivo;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.CreatePlanPreventivoManoObra;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.DeletePlanPreventivoManoObra;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.GetPlanPreventivoManoObraByPlan;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.UpdatePlanPreventivoManoObra;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.CreatePlanPreventivoMaterial;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.DeletePlanPreventivoMaterial;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.GetPlanPreventivoMaterialByPlan;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.UpdatePlanPreventivoMaterial;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Maestros
{
    public static class PlanesPreventivoEndpoints
    {
        public static void MapPlanesPreventivoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/maestros/planes-preventivo")
                .WithTags("Planes de Mantenimiento Preventivo")
                .RequireAuthorization();

            // ---------------- Plan ----------------
            group.MapPost("/", async (CreatePlanPreventivoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/maestros/planes-preventivo/{result.PlantaCode}/{result.Code}", result);
            })
            .WithName("CreatePlanPreventivo")
            .RequireAuthorization("PLANESPREVENTIVO.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetPlanesPreventivoPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetPlanesPreventivoPagedQueryString")
            .RequireAuthorization("PLANESPREVENTIVO.VIEW");

            group.MapPost("/paged/body", async (GetPlanesPreventivoPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetPlanesPreventivoPagedBody")
            .RequireAuthorization("PLANESPREVENTIVO.VIEW");

            group.MapGet("/{plantaCode}/{code}", async (string plantaCode, string code, ISender sender) =>
                Results.Ok(await sender.Send(new GetPlanPreventivoByKeyQuery(plantaCode, code))))
            .WithName("GetPlanPreventivoByKey")
            .RequireAuthorization("PLANESPREVENTIVO.VIEW");

            group.MapPut("/{plantaCode}/{code}", async (
                string plantaCode, string code, UpdatePlanPreventivoCommand command, ISender sender) =>
            {
                if (!string.Equals(plantaCode, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(code, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdatePlanPreventivo")
            .RequireAuthorization("PLANESPREVENTIVO.UPDATE");

            // Copia el plan con su mano de obra y materiales bajo un código nuevo; devuelve el plan nuevo.
            group.MapPost("/{plantaCode}/{code}/copiar", async (string plantaCode, string code, ISender sender) =>
            {
                var result = await sender.Send(new CopyPlanPreventivoCommand(plantaCode, code));
                return Results.Created($"/api/mantenimiento/maestros/planes-preventivo/{result.PlantaCode}/{result.Code}", result);
            })
            .WithName("CopyPlanPreventivo")
            .RequireAuthorization("PLANESPREVENTIVO.CREATE");

            group.MapDelete("/{plantaCode}/{code}", async (string plantaCode, string code, ISender sender) =>
            {
                await sender.Send(new DeletePlanPreventivoCommand(plantaCode, code));
                return Results.NoContent();
            })
            .WithName("DeletePlanPreventivo")
            .RequireAuthorization("PLANESPREVENTIVO.DELETE");

            // ---------------- Mano de obra ----------------
            group.MapGet("/{plantaCode}/{code}/mano-obra", async (string plantaCode, string code, ISender sender) =>
                Results.Ok(await sender.Send(new GetPlanPreventivoManoObraByPlanQuery(plantaCode, code))))
            .WithName("GetPlanPreventivoManoObraByPlan")
            .RequireAuthorization("PLANESPREVENTIVO.VIEW");

            group.MapPost("/{plantaCode}/{code}/mano-obra", async (
                string plantaCode, string code, CreatePlanPreventivoManoObraCommand command, ISender sender) =>
            {
                if (!string.Equals(plantaCode, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(code, command.PlanCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/maestros/planes-preventivo/{plantaCode}/{code}/mano-obra/{result.TrabajadorCode}", result);
            })
            .WithName("CreatePlanPreventivoManoObra")
            .RequireAuthorization("PLANESPREVENTIVO.UPDATE");

            group.MapPut("/{plantaCode}/{code}/mano-obra/{trabajadorCode}", async (
                string plantaCode, string code, string trabajadorCode,
                UpdatePlanPreventivoManoObraCommand command, ISender sender) =>
            {
                if (!string.Equals(plantaCode, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(code, command.PlanCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(trabajadorCode, command.TrabajadorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdatePlanPreventivoManoObra")
            .RequireAuthorization("PLANESPREVENTIVO.UPDATE");

            group.MapDelete("/{plantaCode}/{code}/mano-obra/{trabajadorCode}", async (
                string plantaCode, string code, string trabajadorCode, ISender sender) =>
            {
                await sender.Send(new DeletePlanPreventivoManoObraCommand(plantaCode, code, trabajadorCode));
                return Results.NoContent();
            })
            .WithName("DeletePlanPreventivoManoObra")
            .RequireAuthorization("PLANESPREVENTIVO.UPDATE");

            // ---------------- Materiales ----------------
            group.MapGet("/{plantaCode}/{code}/materiales", async (string plantaCode, string code, ISender sender) =>
                Results.Ok(await sender.Send(new GetPlanPreventivoMaterialByPlanQuery(plantaCode, code))))
            .WithName("GetPlanPreventivoMaterialByPlan")
            .RequireAuthorization("PLANESPREVENTIVO.VIEW");

            group.MapPost("/{plantaCode}/{code}/materiales", async (
                string plantaCode, string code, CreatePlanPreventivoMaterialCommand command, ISender sender) =>
            {
                if (!string.Equals(plantaCode, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(code, command.PlanCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/maestros/planes-preventivo/{plantaCode}/{code}/materiales/{result.ArticuloCode}", result);
            })
            .WithName("CreatePlanPreventivoMaterial")
            .RequireAuthorization("PLANESPREVENTIVO.UPDATE");

            group.MapPut("/{plantaCode}/{code}/materiales/{articuloCode}", async (
                string plantaCode, string code, string articuloCode,
                UpdatePlanPreventivoMaterialCommand command, ISender sender) =>
            {
                if (!string.Equals(plantaCode, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(code, command.PlanCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(articuloCode, command.ArticuloCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdatePlanPreventivoMaterial")
            .RequireAuthorization("PLANESPREVENTIVO.UPDATE");

            group.MapDelete("/{plantaCode}/{code}/materiales/{articuloCode}", async (
                string plantaCode, string code, string articuloCode, ISender sender) =>
            {
                await sender.Send(new DeletePlanPreventivoMaterialCommand(plantaCode, code, articuloCode));
                return Results.NoContent();
            })
            .WithName("DeletePlanPreventivoMaterial")
            .RequireAuthorization("PLANESPREVENTIVO.UPDATE");
        }
    }
}
