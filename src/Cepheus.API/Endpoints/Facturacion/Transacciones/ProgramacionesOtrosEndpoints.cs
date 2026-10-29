using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.AnularProgramacionOtros;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.CerrarProgramacionOtros;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.CreateProgramacionOtros;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.GetLineasDisponiblesProgramacionOtros;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.GetProgramacionesOtrosPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.GetProgramacionOtrosByCode;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.UpdateProgramacionOtros;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones;

public static class ProgramacionesOtrosEndpoints
{
    private const string Base = "/api/facturacion/transacciones/programaciones-otros";

    public static void MapProgramacionesOtrosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Base)
            .WithTags("Programaciones de Otros (Facturación)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateProgramacionOtrosCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"{Base}/{result.Code}", result);
        })
        .WithName("CreateProgramacionOtros")
        .RequireAuthorization("PROGRAMACIONESOTROS.CREATE");

        group.MapGet("/paged", async ([AsParameters] GetProgramacionesOtrosPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetProgramacionesOtrosPaginated")
        .RequireAuthorization("PROGRAMACIONESOTROS.VIEW");

        group.MapGet("/{code}", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new GetProgramacionOtrosByCodeQuery(code))))
        .WithName("GetProgramacionOtrosByCode")
        .RequireAuthorization("PROGRAMACIONESOTROS.VIEW");

        group.MapGet("/ordenes-trabajo/{otCode}/lineas-disponibles", async (string otCode, ISender sender) =>
            Results.Ok(await sender.Send(new GetLineasDisponiblesProgramacionOtrosQuery(otCode))))
        .WithName("GetLineasDisponiblesProgramacionOtros")
        .RequireAuthorization("PROGRAMACIONESOTROS.VIEW");

        group.MapPut("/{code}", async (string code, UpdateProgramacionOtrosRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateProgramacionOtrosCommand(code, body))))
        .WithName("UpdateProgramacionOtros")
        .RequireAuthorization("PROGRAMACIONESOTROS.UPDATE");

        group.MapDelete("/{code}", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new AnularProgramacionOtrosCommand(code))))
        .WithName("AnularProgramacionOtros")
        .RequireAuthorization("PROGRAMACIONESOTROS.ANNUL");

        group.MapPost("/{code}/cerrar", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new CerrarProgramacionOtrosCommand(code))))
        .WithName("CerrarProgramacionOtros")
        .RequireAuthorization("PROGRAMACIONESOTROS.CLOSE");
    }
}
