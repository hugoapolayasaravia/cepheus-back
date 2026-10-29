using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.CreateMotivoChequeRebote;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.GetMotivoChequeReboteByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.GetMotivosChequeRebotePaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.ToggleMotivoChequeReboteStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.UpdateMotivoChequeRebote;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class MotivosChequeReboteEndpoints
    {
        private const string Route = "/api/facturacion/catalogos/motivos-cheque-rebote";

        public static void MapMotivosChequeReboteEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Motivos de Rebote de Cheque (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateMotivoChequeReboteCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"{Route}/{result.Code}", result);
            })
            .WithName("CreateMotivoChequeRebote")
            .RequireAuthorization("MOTIVOSCHEQUEREBOTE.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetMotivosChequeRebotePaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetMotivosChequeRebotePagedQueryString")
            .RequireAuthorization("MOTIVOSCHEQUEREBOTE.VIEW");

            group.MapPost("/paged/body", async (GetMotivosChequeRebotePaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetMotivosChequeRebotePagedBody")
            .RequireAuthorization("MOTIVOSCHEQUEREBOTE.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new GetMotivoChequeReboteByCodeQuery(codigo))))
            .WithName("GetMotivoChequeReboteByCode")
            .RequireAuthorization("MOTIVOSCHEQUEREBOTE.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateMotivoChequeReboteCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateMotivoChequeRebote")
            .RequireAuthorization("MOTIVOSCHEQUEREBOTE.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
                Results.Ok(new { IsActive = await sender.Send(new ToggleMotivoChequeReboteStatusCommand(codigo)) }))
            .WithName("ToggleMotivoChequeReboteStatus")
            .RequireAuthorization("MOTIVOSCHEQUEREBOTE.UPDATE");
        }
    }
}
