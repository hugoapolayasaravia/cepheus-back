using Cepheus.Application.Features.Logistica.Transacciones.Kardex.GetKardex;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones;

public static class KardexEndpoints
{
    public static void MapKardexEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/transacciones/kardex")
            .WithTags("Kardex (Logística)")
            .RequireAuthorization();

        // Kardex (Movimiento de Artículos) de un artículo en una planta, entre dos fechas. Solo lectura.
        group.MapGet("/", async (
            string plantaCode, string articuloCode, DateTime fechaDesde, DateTime fechaHasta, ISender sender) =>
                Results.Ok(await sender.Send(new GetKardexQuery(plantaCode, articuloCode, fechaDesde, fechaHasta))))
        .WithName("GetKardex")
        .RequireAuthorization("KARDEX.VIEW");
    }
}
