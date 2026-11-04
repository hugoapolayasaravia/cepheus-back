using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.CreateFormaPagoCancelacion;
using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.GetFormaPagoCancelacionById;
using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.GetFormasPagoCancelacionPaginated;
using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.ToggleFormaPagoCancelacionStatus;
using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.UpdateFormaPagoCancelacion;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Catalogos
{
    public static class FormasPagoCancelacionEndpoints
    {
        public static void MapFormasPagoCancelacionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/catalogos/formas-pago-cancelacion")
                .WithTags("Formas de pago de cancelación")
                .RequireAuthorization();

            group.MapPost("/", async (CreateFormaPagoCancelacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/catalogos/formas-pago-cancelacion/{result.Code}", result);
            })
            .WithName("CreateFormaPagoCancelacion")
            .RequireAuthorization("FORMASPAGOCANCELA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetFormasPagoCancelacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFormasPagoCancelacionPagedQueryString")
            .RequireAuthorization("FORMASPAGOCANCELA.VIEW");

            group.MapPost("/paged/body", async (
                GetFormasPagoCancelacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFormasPagoCancelacionPagedBody")
            .RequireAuthorization("FORMASPAGOCANCELA.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetFormaPagoCancelacionByIdQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetFormaPagoCancelacionById")
            .RequireAuthorization("FORMASPAGOCANCELA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateFormaPagoCancelacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateFormaPagoCancelacion")
            .RequireAuthorization("FORMASPAGOCANCELA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleFormaPagoCancelacionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleFormaPagoCancelacionStatus")
            .RequireAuthorization("FORMASPAGOCANCELA.UPDATE");
        }
    }
}
