using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.CreateCuentaBancaria;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.GetCuentaBancariaByCode;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.GetCuentasBancariasPaginated;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.ToggleCuentaBancariaStatus;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.UpdateCuentaBancaria;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Maestros
{
    public static class CuentasBancariasEndpoints
    {
        public static void MapCuentasBancariasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/maestros/cuentas-bancarias")
                .WithTags("Cuentas bancarias")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCuentaBancariaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/maestros/cuentas-bancarias/{result.BankCode}/{result.AccountNumber}", result);
            })
            .WithName("CreateCuentaBancaria")
            .RequireAuthorization("CUENTASBANCARIAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCuentasBancariasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCuentasBancariasPagedQueryString")
            .RequireAuthorization("CUENTASBANCARIAS.VIEW");

            group.MapPost("/paged/body", async (
                GetCuentasBancariasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCuentasBancariasPagedBody")
            .RequireAuthorization("CUENTASBANCARIAS.VIEW");

            group.MapGet("/{bankCode}/{accountNumber}", async (string bankCode, string accountNumber, ISender sender) =>
            {
                var result = await sender.Send(new GetCuentaBancariaByCodeQuery(bankCode, accountNumber));
                return Results.Ok(result);
            })
            .WithName("GetCuentaBancariaByCode")
            .RequireAuthorization("CUENTASBANCARIAS.VIEW");

            group.MapPut("/{bankCode}/{accountNumber}", async (string bankCode, string accountNumber, UpdateCuentaBancariaCommand command, ISender sender) =>
            {
                if (!string.Equals(bankCode, command.BankCode, StringComparison.OrdinalIgnoreCase) || !string.Equals(accountNumber, command.AccountNumber, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCuentaBancaria")
            .RequireAuthorization("CUENTASBANCARIAS.UPDATE");

            group.MapPatch("/{bankCode}/{accountNumber}/toggle-status", async (string bankCode, string accountNumber, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleCuentaBancariaStatusCommand(bankCode, accountNumber));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleCuentaBancariaStatus")
            .RequireAuthorization("CUENTASBANCARIAS.UPDATE");
        }
    }
}
