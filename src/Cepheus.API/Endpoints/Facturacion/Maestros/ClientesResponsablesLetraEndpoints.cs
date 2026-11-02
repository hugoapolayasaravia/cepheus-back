using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.CreateClienteResponsableLetra;
using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.DeleteClienteResponsableLetra;
using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.GetClientesResponsablesLetra;
using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.UpdateClienteResponsableLetra;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    /// <summary>
    /// Responsables / avales de letras de un cliente (legacy MClientes_Responsables_Letras). Se eligen al imprimir
    /// una letra. Clave: /{cliente}/{secuencia}; la secuencia (001, 002, ...) la asigna el sistema.
    /// </summary>
    public static class ClientesResponsablesLetraEndpoints
    {
        private const string Route = "/api/facturacion/maestros/clientes-responsables-letra";

        public record ClienteResponsableLetraBody(
            string? PaternalSurname, string? MaternalSurname, string? FirstNames,
            string? DocumentTypeCode, string? DocumentNumber, string? Address, string? Phone);

        public static void MapClientesResponsablesLetraEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Clientes - Responsables de letra (Facturación)")
                .RequireAuthorization();

            group.MapGet("/{cliente}", async (string cliente, ISender sender) =>
                Results.Ok(await sender.Send(new GetClientesResponsablesLetraQuery(cliente))))
            .WithName("GetClientesResponsablesLetra")
            .RequireAuthorization("CLIENTESRESPONSABLESLETRA.VIEW");

            group.MapPost("/{cliente}", async (string cliente, ClienteResponsableLetraBody b, ISender sender) =>
            {
                var result = await sender.Send(new CreateClienteResponsableLetraCommand(
                    cliente, b.PaternalSurname, b.MaternalSurname, b.FirstNames,
                    b.DocumentTypeCode, b.DocumentNumber, b.Address, b.Phone));
                return Results.Created($"{Route}/{cliente}/{result.Sequence}", result);
            })
            .WithName("CreateClienteResponsableLetra")
            .RequireAuthorization("CLIENTESRESPONSABLESLETRA.CREATE");

            group.MapPut("/{cliente}/{secuencia}", async (
                string cliente, string secuencia, ClienteResponsableLetraBody b, ISender sender) =>
                Results.Ok(await sender.Send(new UpdateClienteResponsableLetraCommand(
                    cliente, secuencia, b.PaternalSurname, b.MaternalSurname, b.FirstNames,
                    b.DocumentTypeCode, b.DocumentNumber, b.Address, b.Phone))))
            .WithName("UpdateClienteResponsableLetra")
            .RequireAuthorization("CLIENTESRESPONSABLESLETRA.UPDATE");

            group.MapDelete("/{cliente}/{secuencia}", async (string cliente, string secuencia, ISender sender) =>
            {
                await sender.Send(new DeleteClienteResponsableLetraCommand(cliente, secuencia));
                return Results.NoContent();
            })
            .WithName("DeleteClienteResponsableLetra")
            .RequireAuthorization("CLIENTESRESPONSABLESLETRA.DELETE");
        }
    }
}
