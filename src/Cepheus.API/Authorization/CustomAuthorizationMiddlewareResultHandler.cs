using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;

namespace Cepheus.API.Authorization;

public class CustomAuthorizationMiddlewareResultHandler
    : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Acceso denegado",
                Detail = "No tiene acceso por permisos, consulte con el administrador.",
                Instance = context.Request.Path,
                Type = "https://httpstatuses.com/403"
            };

            await context.Response.WriteAsJsonAsync(problemDetails);

            return;
        }

        await _defaultHandler.HandleAsync(
            next,
            context,
            policy,
            authorizeResult);
    }
}