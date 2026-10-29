using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.CreateArticuloStock;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.DecreaseArticuloStock;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.GetArticuloStock;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.GetStockArticulosPaginated;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.IncreaseArticuloStock;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.UpdateArticuloStockThresholds;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class StockArticulosEndpoints
    {
        public static void MapStockArticulosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/stock-articulos")
                .WithTags("Stock de Artículos (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateArticuloStockCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/maestros/stock-articulos/{result.PlantaCode}/{result.ArticuloCode}", result);
            })
            .WithName("CreateArticuloStock")
            .RequireAuthorization("STOCKARTICULOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetStockArticulosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetStockArticulosPagedQueryString")
            .RequireAuthorization("STOCKARTICULOS.VIEW");

            group.MapPost("/paged/body", async (
                GetStockArticulosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetStockArticulosPagedBody")
            .RequireAuthorization("STOCKARTICULOS.VIEW");

            group.MapGet("/{codigoPlanta}/{codigoArticulo}", async (string codigoPlanta, string codigoArticulo, ISender sender) =>
            {
                var result = await sender.Send(new GetArticuloStockQuery(codigoPlanta, codigoArticulo));
                return Results.Ok(result);
            })
            .WithName("GetArticuloStock")
            .RequireAuthorization("STOCKARTICULOS.VIEW");

            group.MapPost("/{codigoPlanta}/{codigoArticulo}/incrementar", async (
                string codigoPlanta, string codigoArticulo, decimal cantidad, ISender sender) =>
            {
                var result = await sender.Send(new IncreaseArticuloStockCommand(codigoPlanta, codigoArticulo, cantidad));
                return Results.Ok(result);
            })
            .WithName("IncreaseArticuloStock")
            .RequireAuthorization("STOCKARTICULOS.MOVE");

            group.MapPost("/{codigoPlanta}/{codigoArticulo}/reducir", async (
                string codigoPlanta, string codigoArticulo, decimal cantidad, ISender sender) =>
            {
                var result = await sender.Send(new DecreaseArticuloStockCommand(codigoPlanta, codigoArticulo, cantidad));
                return Results.Ok(result);
            })
            .WithName("DecreaseArticuloStock")
            .RequireAuthorization("STOCKARTICULOS.MOVE");

            group.MapPut("/{codigoPlanta}/{codigoArticulo}/umbrales", async (
                string codigoPlanta, string codigoArticulo, UpdateArticuloStockThresholdsCommand command, ISender sender) =>
            {
                if (codigoPlanta != command.PlantaCode || codigoArticulo != command.ArticuloCode)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateArticuloStockThresholds")
            .RequireAuthorization("STOCKARTICULOS.UPDATE");
        }
    }

}
