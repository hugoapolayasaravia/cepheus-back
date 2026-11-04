using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.CastigarCobranzaDudosa;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.DeleteCobranzaDudosa;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GenerarCanjeCobranzaDudosa;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GetCobranzaDudosa;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GetCobranzasDudosasPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.RegistrarCobranzaDudosa;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    /// <summary>
    /// Cobranza dudosa de documentos de venta (legacy w_con_cobranzadudosa). El documento se identifica por
    /// /{typeCode}/{number} y, según el tipo, ?plantaCode= (08, 10) o ?clienteCode=&amp;bancoCode= (cheques 11).
    /// Flujo: registrar -> generar asiento de canje -> (opcional) castigar. Eliminar deshace el proceso.
    /// Los documentos candidatos se buscan en /api/facturacion/transacciones/cobranzas/documentos-pendientes.
    /// </summary>
    public static class CobranzasDudosasEndpoints
    {
        private const string Route = "/api/facturacion/transacciones/cobranzas-dudosas";

        public record RowVersionBody(byte[] RowVersion);

        public static void MapCobranzasDudosasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Cobranza Dudosa (Facturación)")
                .RequireAuthorization();

            group.MapGet("/paged", async ([AsParameters] GetCobranzasDudosasPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetCobranzasDudosasPagedQueryString")
            .RequireAuthorization("COBRANZASDUDOSAS.VIEW");

            group.MapPost("/paged/body", async (GetCobranzasDudosasPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetCobranzasDudosasPagedBody")
            .RequireAuthorization("COBRANZASDUDOSAS.VIEW");

            group.MapGet("/{typeCode}/{number}", async (
                string typeCode, string number, string? plantaCode, string? clienteCode, string? bancoCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetCobranzaDudosaQuery(typeCode, plantaCode, number, clienteCode, bancoCode))))
            .WithName("GetCobranzaDudosa")
            .RequireAuthorization("COBRANZASDUDOSAS.VIEW");

            group.MapPost("/", async (RegistrarCobranzaDudosaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"{Route}/{result.TypeCode}/{result.Number}", result);
            })
            .WithName("RegistrarCobranzaDudosa")
            .RequireAuthorization("COBRANZASDUDOSAS.CREATE");

            group.MapPost("/{typeCode}/{number}/generar-canje", async (
                string typeCode, string number, string? plantaCode, string? clienteCode, string? bancoCode,
                RowVersionBody body, ISender sender) =>
                Results.Ok(await sender.Send(new GenerarCanjeCobranzaDudosaCommand(
                    typeCode, plantaCode, number, clienteCode, bancoCode, body.RowVersion))))
            .WithName("GenerarCanjeCobranzaDudosa")
            .RequireAuthorization("COBRANZASDUDOSAS.GENERAR");

            group.MapPost("/{typeCode}/{number}/castigar", async (
                string typeCode, string number, string? plantaCode, string? clienteCode, string? bancoCode,
                RowVersionBody body, ISender sender) =>
                Results.Ok(await sender.Send(new CastigarCobranzaDudosaCommand(
                    typeCode, plantaCode, number, clienteCode, bancoCode, body.RowVersion))))
            .WithName("CastigarCobranzaDudosa")
            .RequireAuthorization("COBRANZASDUDOSAS.CASTIGAR");

            group.MapDelete("/{typeCode}/{number}", async (
                string typeCode, string number, string? plantaCode, string? clienteCode, string? bancoCode, ISender sender) =>
            {
                await sender.Send(new DeleteCobranzaDudosaCommand(typeCode, plantaCode, number, clienteCode, bancoCode));
                return Results.NoContent();
            })
            .WithName("DeleteCobranzaDudosa")
            .RequireAuthorization("COBRANZASDUDOSAS.DELETE");
        }
    }
}
