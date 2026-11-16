using Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.GetAprobadoresDocumento;
using Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.GetPendientes;
using Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.GetResumenPendientes;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones
{
    /// <summary>
    /// Bandeja de aprobación (legacy: logistica_w / w_log_aprobacion_tra).
    /// Aprobar / desaprobar se hace con los PATCH .../estado de cada documento (pedidos, ordenes-compra,
    /// ordenes-servicio, vales), que validan al aprobador con la misma regla.
    /// Permisos (Programa "APROBACIONES"): VIEW = bandeja propia; VIEW_ALL = ver todo lo pendiente.
    /// </summary>
    public static class AprobacionesEndpoints
    {
        public static void MapAprobacionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/transacciones/aprobaciones")
                .WithTags("Aprobaciones (Logística)")
                .RequireAuthorization();

            // Mi bandeja: cantidad de pendientes por tipo.
            group.MapGet("/resumen", async (ISender sender) =>
            {
                var result = await sender.Send(new GetResumenPendientesQuery(Todos: false));
                return Results.Ok(result);
            })
            .WithName("GetAprobacionesResumen")
            .RequireAuthorization("APROBACIONES.VIEW");

            // Todo lo pendiente (legacy: Usuario_Res = 'T').
            group.MapGet("/resumen/todos", async (ISender sender) =>
            {
                var result = await sender.Send(new GetResumenPendientesQuery(Todos: true));
                return Results.Ok(result);
            })
            .WithName("GetAprobacionesResumenTodos")
            .RequireAuthorization("APROBACIONES.VIEW");

            // Mis pendientes de un tipo: SC, OC, OS, VS, VC.
            group.MapGet("/{tipo}/pendientes", async (
                string tipo,
                [AsParameters] GetPendientesPaginadoParams p,
                ISender sender) =>
            {
                var result = await sender.Send(p.ToQuery(tipo, todos: false));
                return Results.Ok(result);
            })
            .WithName("GetAprobacionesPendientes")
            .RequireAuthorization("APROBACIONES.VIEW");

            group.MapGet("/{tipo}/pendientes/todos", async (
                string tipo,
                [AsParameters] GetPendientesPaginadoParams p,
                ISender sender) =>
            {
                var result = await sender.Send(p.ToQuery(tipo, todos: true));
                return Results.Ok(result);
            })
            .WithName("GetAprobacionesPendientesTodos")
            .RequireAuthorization("APROBACIONES.VIEW");

            // Quiénes pueden aprobar hoy un documento.
            group.MapGet("/{tipo}/{codigoPlanta}/{codigo}/aprobadores", async (
                string tipo, string codigoPlanta, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetAprobadoresDocumentoQuery(tipo, codigoPlanta, codigo));
                return Results.Ok(result);
            })
            .WithName("GetAprobadoresDocumento")
            .RequireAuthorization("APROBACIONES.VIEW");
        }
    }

    /// <summary>Parámetros de query string del listado (PagedRequest es abstracta y Tipo/Todos vienen de la ruta).</summary>
    public sealed class GetPendientesPaginadoParams
    {
        public string? Search { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public GetPendientesQuery ToQuery(string tipo, bool todos) => new()
        {
            Tipo = tipo,
            Todos = todos,
            Search = Search,
            PageNumber = PageNumber,
            PageSize = PageSize
        };
    }
}
