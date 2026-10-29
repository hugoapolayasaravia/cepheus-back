using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.CreateArticuloAnexo;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.DeleteArticuloAnexo;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.GetArticuloAnexoByKey;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.GetArticuloAnexosPaginated;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.UpdateArticuloAnexo;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ArticuloAnexosEndpoints
    {
        public static void MapArticuloAnexosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/articulo-anexos")
                .WithTags("Artículo - Anexo contable por subcentro (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateArticuloAnexoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/articulo-anexos/{result.SubCentroCostoCode}/{result.ArticuloCode}", result);
            })
            .WithName("CreateArticuloAnexo")
            .RequireAuthorization("ARTICULOANEXOS.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetArticuloAnexosPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetArticuloAnexosPagedQueryString")
            .RequireAuthorization("ARTICULOANEXOS.VIEW");

            group.MapPost("/paged/body", async (GetArticuloAnexosPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetArticuloAnexosPagedBody")
            .RequireAuthorization("ARTICULOANEXOS.VIEW");

            group.MapGet("/{subCentroCostoCode}/{articuloCode}", async (
                string subCentroCostoCode, string articuloCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetArticuloAnexoByKeyQuery(subCentroCostoCode, articuloCode))))
            .WithName("GetArticuloAnexoByKey")
            .RequireAuthorization("ARTICULOANEXOS.VIEW");

            group.MapPut("/{subCentroCostoCode}/{articuloCode}", async (
                string subCentroCostoCode, string articuloCode, UpdateArticuloAnexoCommand command, ISender sender) =>
            {
                if (!string.Equals(subCentroCostoCode, command.SubCentroCostoCode, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(articuloCode, command.ArticuloCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateArticuloAnexo")
            .RequireAuthorization("ARTICULOANEXOS.UPDATE");

            group.MapDelete("/{subCentroCostoCode}/{articuloCode}", async (
                string subCentroCostoCode, string articuloCode, ISender sender) =>
            {
                await sender.Send(new DeleteArticuloAnexoCommand(subCentroCostoCode, articuloCode));
                return Results.NoContent();
            })
            .WithName("DeleteArticuloAnexo")
            .RequireAuthorization("ARTICULOANEXOS.DELETE");
        }
    }
}
