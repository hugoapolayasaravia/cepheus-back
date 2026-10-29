using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.AnularProgramacionConcreto;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.CerrarProgramacionConcreto;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.CreateProgramacionConcreto;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.GetLineasDisponiblesProgramacionConcreto;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.GetProgramacionConcretoByCode;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.GetProgramacionesConcretoPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.UpdateProgramacionConcreto;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones;

public static class ProgramacionesConcretoEndpoints
{
    private const string Base = "/api/facturacion/transacciones/programaciones-concreto";

    public static void MapProgramacionesConcretoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Base)
            .WithTags("Programaciones de Concreto (Facturación)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateProgramacionConcretoCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"{Base}/{result.Code}", result);
        })
        .WithName("CreateProgramacionConcreto")
        .RequireAuthorization("PROGRAMACIONESCONCRETO.CREATE");

        group.MapGet("/paged", async ([AsParameters] GetProgramacionesConcretoPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetProgramacionesConcretoPaginated")
        .RequireAuthorization("PROGRAMACIONESCONCRETO.VIEW");

        group.MapGet("/{code}", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new GetProgramacionConcretoByCodeQuery(code))))
        .WithName("GetProgramacionConcretoByCode")
        .RequireAuthorization("PROGRAMACIONESCONCRETO.VIEW");

        group.MapGet("/ordenes-trabajo/{otCode}/lineas-disponibles", async (string otCode, ISender sender) =>
            Results.Ok(await sender.Send(new GetLineasDisponiblesProgramacionConcretoQuery(otCode))))
        .WithName("GetLineasDisponiblesProgramacionConcreto")
        .RequireAuthorization("PROGRAMACIONESCONCRETO.VIEW");

        group.MapPut("/{code}", async (string code, UpdateProgramacionConcretoRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateProgramacionConcretoCommand(code, body))))
        .WithName("UpdateProgramacionConcreto")
        .RequireAuthorization("PROGRAMACIONESCONCRETO.UPDATE");

        group.MapDelete("/{code}", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new AnularProgramacionConcretoCommand(code))))
        .WithName("AnularProgramacionConcreto")
        .RequireAuthorization("PROGRAMACIONESCONCRETO.ANNUL");

        group.MapPost("/{code}/cerrar", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new CerrarProgramacionConcretoCommand(code))))
        .WithName("CerrarProgramacionConcreto")
        .RequireAuthorization("PROGRAMACIONESCONCRETO.CLOSE");
    }
}
