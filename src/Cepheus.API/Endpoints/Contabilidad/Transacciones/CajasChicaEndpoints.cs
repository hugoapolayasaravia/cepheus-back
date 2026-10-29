using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.AnularCajaChica;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.CancelarCajaChica;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.ProvisionarCajaChica;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.ReabrirCajaChica;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.CreateCajaChica;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.GetCajaChicaByCode;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.GetCajasChicaPaginated;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.ImprimirCajaChica;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.UpdateCajaChica;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Transacciones
{
    public static class CajasChicaEndpoints
    {
        private const string Route = "/api/contabilidad/transacciones/cajas-chica";

        public static void MapCajasChicaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Cajas Chica (Contabilidad)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCajaChicaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"{Route}/{result.Code}", result);
            })
            .WithName("CreateCajaChica")
            .RequireAuthorization("CAJASCHICA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCajasChicaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCajasChicaPagedQueryString")
            .RequireAuthorization("CAJASCHICA.VIEW");

            group.MapPost("/paged/body", async (
                GetCajasChicaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCajasChicaPagedBody")
            .RequireAuthorization("CAJASCHICA.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCajaChicaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetCajaChicaByCode")
            .RequireAuthorization("CAJASCHICA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateCajaChicaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCajaChica")
            .RequireAuthorization("CAJASCHICA.UPDATE");

            group.MapPatch("/{codigo}/impresion", async (string codigo, ImprimirCajaChicaRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(new ImprimirCajaChicaCommand(codigo, body.Printed));
                return Results.Ok(result);
            })
            .WithName("ImprimirCajaChica")
            .RequireAuthorization("CAJASCHICA.UPDATE");

            group.MapPatch("/{codigo}/anular", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new AnularCajaChicaCommand(codigo));
                return Results.Ok(result);
            })
            .WithName("AnularCajaChica")
            .RequireAuthorization("CAJASCHICA.ANULAR");

            group.MapPost("/{codigo}/provisionar", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new ProvisionarCajaChicaCommand(codigo));
                return Results.Ok(result);
            })
            .WithName("ProvisionarCajaChica")
            .RequireAuthorization("CAJASCHICA.PROVISIONAR");

            group.MapPost("/{codigo}/reabrir", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new ReabrirCajaChicaCommand(codigo));
                return Results.Ok(result);
            })
            .WithName("ReabrirCajaChica")
            .RequireAuthorization("CAJASCHICA.REABRIR");

            group.MapPost("/{codigo}/cancelar", async (string codigo, CancelarCajaChicaRequestBody? body, ISender sender) =>
            {
                var result = await sender.Send(new CancelarCajaChicaCommand(codigo, body?.ReversionYear, body?.ReversionMonth));
                return Results.Ok(result);
            })
            .WithName("CancelarCajaChica")
            .RequireAuthorization("CAJASCHICA.CANCELAR");
        }
    }

    public record ImprimirCajaChicaRequestBody(bool Printed);

    public record CancelarCajaChicaRequestBody(string? ReversionYear, string? ReversionMonth);
}
