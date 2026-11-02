using Cepheus.Application.Features.Facturacion.Transacciones.Cobranzas.GetDocumentosPendientes;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    /// <summary>Consultas compartidas de cobranza (Letras, Cancelaciones).</summary>
    public static class CobranzasEndpoints
    {
        private const string Route = "/api/facturacion/transacciones/cobranzas";

        public static void MapCobranzasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Cobranzas (Facturación)")
                .RequireAuthorization();

            group.MapGet("/documentos-pendientes", async (
                [AsParameters] GetDocumentosPendientesQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetDocumentosPendientes")
            .RequireAuthorization("COBRANZAS.VIEW");
        }
    }
}
