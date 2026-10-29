using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.GetOrdenesServicioSalidaPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.GetOrdenServicioSalidaByCode;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.UpdateOrdenServicioSalida;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones;

/// <summary>
/// Vale de salida de la Orden de Servicio. No se crea ni se anula por separado: nace con la orden, sus líneas
/// se sincronizan con las de la orden y cambia de estado cuando la orden se procesa o se anula. Comparte los
/// permisos de la Orden de Servicio.
/// </summary>
public static class OrdenesServicioSalidaEndpoints
{
    public static void MapOrdenesServicioSalidaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/transacciones/ordenes-servicio-salida")
            .WithTags("Vales de salida de Orden de Servicio (Logística)")
            .RequireAuthorization();

        group.MapGet("/paged", async ([AsParameters] GetOrdenesServicioSalidaPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetOrdenesServicioSalidaPagedQueryString")
        .RequireAuthorization("ORDENESSERVICIO.VIEW");

        group.MapPost("/paged/body", async (GetOrdenesServicioSalidaPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetOrdenesServicioSalidaPagedBody")
        .RequireAuthorization("ORDENESSERVICIO.VIEW");

        group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
            Results.Ok(await sender.Send(new GetOrdenServicioSalidaByCodeQuery(codigoPlanta, codigo))))
        .WithName("GetOrdenServicioSalidaByCode")
        .RequireAuthorization("ORDENESSERVICIO.VIEW");

        group.MapPut("/{codigoPlanta}/{codigo}", async (
            string codigoPlanta, string codigo, UpdateOrdenServicioSalidaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateOrdenServicioSalidaCommand
            {
                PlantaCode = codigoPlanta,
                Code = codigo,
                TrabajadorCode = body.TrabajadorCode
            })))
        .WithName("UpdateOrdenServicioSalida")
        .RequireAuthorization("ORDENESSERVICIO.UPDATE");
    }

    public sealed record UpdateOrdenServicioSalidaRequest(string TrabajadorCode);
}
