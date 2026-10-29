using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.CreateSubFamiliaAnexo;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.DeleteSubFamiliaAnexo;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.GetSubFamiliaAnexo;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.GetSubFamiliaAnexosPaginated;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.UpdateSubFamiliaAnexo;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros;

public static class SubFamiliaAnexosEndpoints
{
    public static void MapSubFamiliaAnexosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/maestros/subfamilia-anexos")
            .WithTags("Anexos por SubFamilia (Logística)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateSubFamiliaAnexoCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created(
                $"/api/logistica/maestros/subfamilia-anexos/{result.SubCentroCostoCode}/{result.SubFamiliaCode}", result);
        })
        .WithName("CreateSubFamiliaAnexo")
        .RequireAuthorization("SUBFAMILIAANEXOS.CREATE");

        group.MapGet("/paged", async ([AsParameters] GetSubFamiliaAnexosPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetSubFamiliaAnexosPagedQueryString")
        .RequireAuthorization("SUBFAMILIAANEXOS.VIEW");

        group.MapPost("/paged/body", async (GetSubFamiliaAnexosPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetSubFamiliaAnexosPagedBody")
        .RequireAuthorization("SUBFAMILIAANEXOS.VIEW");

        group.MapGet("/{subCentroCostoCode}/{subFamiliaCode}", async (
            string subCentroCostoCode, string subFamiliaCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetSubFamiliaAnexoQuery(subCentroCostoCode, subFamiliaCode))))
        .WithName("GetSubFamiliaAnexo")
        .RequireAuthorization("SUBFAMILIAANEXOS.VIEW");

        group.MapPut("/{subCentroCostoCode}/{subFamiliaCode}", async (
            string subCentroCostoCode, string subFamiliaCode, UpdateSubFamiliaAnexoCommand command, ISender sender) =>
        {
            if (!string.Equals(subCentroCostoCode, command.SubCentroCostoCode, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(subFamiliaCode, command.SubFamiliaCode, StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
            }

            return Results.Ok(await sender.Send(command));
        })
        .WithName("UpdateSubFamiliaAnexo")
        .RequireAuthorization("SUBFAMILIAANEXOS.UPDATE");

        group.MapDelete("/{subCentroCostoCode}/{subFamiliaCode}", async (
            string subCentroCostoCode, string subFamiliaCode, ISender sender) =>
        {
            await sender.Send(new DeleteSubFamiliaAnexoCommand(subCentroCostoCode, subFamiliaCode));
            return Results.NoContent();
        })
        .WithName("DeleteSubFamiliaAnexo")
        .RequireAuthorization("SUBFAMILIAANEXOS.DELETE");
    }
}
