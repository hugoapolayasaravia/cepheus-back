using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.CreateListaPrecio;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.GetListaPrecioById;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.GetListasPrecioPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.ToggleListaPrecioStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.UpdateListaPrecio;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class ListasPrecioEndpoints
    {
        public static void MapListasPrecioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/listas-precio")
                .WithTags("Listas de precio")
                .RequireAuthorization();

            group.MapPost("/", async (CreateListaPrecioCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/listas-precio/{result.Id}", result);
            })
            .WithName("CreateListaPrecio")
            .RequireAuthorization("LISTASPRECIO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetListasPrecioPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetListasPrecioPagedQueryString")
            .RequireAuthorization("LISTASPRECIO.VIEW");

            group.MapPost("/paged/body", async (
                GetListasPrecioPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetListasPrecioPagedBody")
            .RequireAuthorization("LISTASPRECIO.VIEW");

            group.MapGet("/{id:long}", async (long id, ISender sender) =>
            {
                var result = await sender.Send(new GetListaPrecioByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetListaPrecioById")
            .RequireAuthorization("LISTASPRECIO.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateListaPrecioCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El identificador de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateListaPrecio")
            .RequireAuthorization("LISTASPRECIO.UPDATE");

            group.MapPatch("/{id:long}/toggle-status", async (long id, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleListaPrecioStatusCommand(id));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleListaPrecioStatus")
            .RequireAuthorization("LISTASPRECIO.UPDATE");
        }
    }
}
