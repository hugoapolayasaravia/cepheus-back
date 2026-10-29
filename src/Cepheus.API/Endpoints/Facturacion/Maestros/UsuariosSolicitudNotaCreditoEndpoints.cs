using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.CreateUsuarioSolicitudNotaCredito;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.GetUsuarioSolicitudNotaCreditoByKey;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.GetUsuariosSolicitudNotaCreditoPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.ToggleUsuarioSolicitudNotaCreditoStatus;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.UpdateUsuarioSolicitudNotaCredito;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class UsuariosSolicitudNotaCreditoEndpoints
    {
        public static void MapUsuariosSolicitudNotaCreditoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/maestros/usuarios-solicitud-nota-credito")
                .WithTags("Usuarios de Solicitud de Nota de Crédito (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateUsuarioSolicitudNotaCreditoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/maestros/usuarios-solicitud-nota-credito/{result.NegocioCode}/{result.StageName}/{result.UserId}",
                    result);
            })
            .WithName("CreateUsuarioSolicitudNotaCredito")
            .RequireAuthorization("USUARIOSSNC.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetUsuariosSolicitudNotaCreditoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUsuariosSolicitudNotaCreditoPagedQueryString")
            .RequireAuthorization("USUARIOSSNC.VIEW");

            group.MapPost("/paged/body", async (
                GetUsuariosSolicitudNotaCreditoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUsuariosSolicitudNotaCreditoPagedBody")
            .RequireAuthorization("USUARIOSSNC.VIEW");

            group.MapGet("/{negocio}/{etapa}/{userId:int}", async (
                string negocio, string etapa, int userId, ISender sender) =>
            {
                var result = await sender.Send(new GetUsuarioSolicitudNotaCreditoByKeyQuery(negocio, etapa, userId));
                return Results.Ok(result);
            })
            .WithName("GetUsuarioSolicitudNotaCreditoByKey")
            .RequireAuthorization("USUARIOSSNC.VIEW");

            group.MapPut("/{negocio}/{etapa}/{userId:int}", async (
                string negocio, string etapa, int userId,
                UpdateUsuarioSolicitudNotaCreditoCommand command, ISender sender) =>
            {
                if (!string.Equals(negocio, command.NegocioCode, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(etapa, command.Stage, StringComparison.OrdinalIgnoreCase)
                    || userId != command.UserId)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateUsuarioSolicitudNotaCredito")
            .RequireAuthorization("USUARIOSSNC.UPDATE");

            group.MapPatch("/{negocio}/{etapa}/{userId:int}/toggle-status", async (
                string negocio, string etapa, int userId, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleUsuarioSolicitudNotaCreditoStatusCommand(negocio, etapa, userId));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleUsuarioSolicitudNotaCreditoStatus")
            .RequireAuthorization("USUARIOSSNC.UPDATE");
        }
    }
}
