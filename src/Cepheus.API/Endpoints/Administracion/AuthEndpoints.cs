using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Features.Administracion.Users.Authenticate;
using Cepheus.Application.Features.Administracion.Users.Common;
using Cepheus.Application.Features.Administracion.Users.ForgotPassword;
using Cepheus.Application.Features.Administracion.Users.Logout;
using Cepheus.Application.Features.Administracion.Users.Me;
using Cepheus.Application.Features.Administracion.Users.Refresh;
using Cepheus.Application.Features.Administracion.Users.Register;
using Cepheus.Application.Features.Administracion.Users.ResetPassword;
using MediatR;

namespace Cepheus.API.Endpoints.Administracion;

public static class AuthEndpoints
{
    // ------------------------------------------------------------------
    // MODO COOKIE HttpOnly (activo): el refresh token viaja en una cookie que
    // JavaScript no puede leer. El access token sigue viajando en el cuerpo.
    //
    // Para volver al modo localStorage (refresh token en el cuerpo): en cada
    // endpoint descomenta el bloque marcado con [LOCALSTORAGE] y comenta el
    // bloque marcado con [COOKIE]. En el frontend, restaura el servicio que
    // quedó comentado en seguridad.service.ts.
    // ------------------------------------------------------------------
    private const string RefreshCookieName = "cepheus_rt";
    private const string RefreshCookiePath = "/api/auth";

    // Cabecera personalizada que se exige en /refresh y /logout (defensa CSRF):
    // un sitio ajeno no puede enviarla sin pasar por el preflight de CORS, y
    // CORS solo permite los orígenes de Cors:AllowedOrigins.
    private const string ClientHeaderName = "X-Cepheus-Client";
    private const string ClientHeaderValue = "web";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // POST /api/auth/register — SOLO bootstrap del primer usuario.
        // Se bloquea automáticamente (409) si ya existe algún usuario en el sistema.
        group.MapPost("/register", async (RegisterCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/api/users/{result.Id}", result);
        })
        .WithName("Register")
        .AllowAnonymous()
        .RequireRateLimiting("AuthRateLimiter");

        // POST /api/auth/login
        // [COOKIE] El refresh token se entrega en una cookie HttpOnly y NO en el cuerpo.
        group.MapPost("/login", async (
            AuthenticateCommand command,
            ISender sender,
            IJwtTokenService jwtTokenService,
            IConfiguration config,
            HttpContext http) =>
        {
            var result = await sender.Send(command);
            SetRefreshCookie(http, config, result.RefreshToken, jwtTokenService.RefreshTokenExpirationDays);
            return Results.Ok(ToSessionResponse(result));
        })
        .WithName("Login")
        .AllowAnonymous()
        .RequireRateLimiting("AuthRateLimiter");

        // [LOCALSTORAGE] Versión anterior: devolvía el refresh token en el cuerpo.
        //group.MapPost("/login", async (AuthenticateCommand command, ISender sender) =>
        //{
        //    var result = await sender.Send(command);
        //    return Results.Ok(result);
        //})
        //.WithName("Login")
        //.AllowAnonymous()
        //.RequireRateLimiting("AuthRateLimiter");

        // POST /api/auth/refresh — renueva el accessToken usando el refreshToken,
        // sin pedir contraseña de nuevo. Rota el refreshToken (revoca el usado,
        // emite uno nuevo).
        // [COOKIE] El refresh token se lee de la cookie y la cookie se renueva.
        group.MapPost("/refresh", async (
            HttpContext http,
            ISender sender,
            IJwtTokenService jwtTokenService,
            IConfiguration config) =>
        {
            if (!HasClientHeader(http))
            {
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            }

            var refreshToken = http.Request.Cookies[RefreshCookieName];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedAccessException("No hay una sesión activa. Inicie sesión nuevamente.");
            }

            try
            {
                var result = await sender.Send(new RefreshTokenCommand(refreshToken));
                SetRefreshCookie(http, config, result.RefreshToken, jwtTokenService.RefreshTokenExpirationDays);
                return Results.Ok(ToSessionResponse(result));
            }
            catch (UnauthorizedAccessException)
            {
                // Token inválido, revocado o expirado: se limpia la cookie y se propaga el 401.
                DeleteRefreshCookie(http, config);
                throw;
            }
        })
        .WithName("RefreshToken")
        .AllowAnonymous();

        // [LOCALSTORAGE] Versión anterior: recibía el refresh token en el cuerpo.
        //group.MapPost("/refresh", async (RefreshTokenCommand command, ISender sender) =>
        //{
        //    var result = await sender.Send(command);
        //    return Results.Ok(result);
        //})
        //.WithName("RefreshToken")
        //.AllowAnonymous();

        // POST /api/auth/logout — revoca el refreshToken de la cookie y la borra.
        // Idempotente: si ya estaba revocado o no existe, igual responde 204.
        // [COOKIE] El refresh token se lee de la cookie.
        group.MapPost("/logout", async (
            HttpContext http,
            ISender sender,
            IConfiguration config) =>
        {
            if (!HasClientHeader(http))
            {
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            }

            var refreshToken = http.Request.Cookies[RefreshCookieName];
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await sender.Send(new LogoutCommand(refreshToken));
            }

            DeleteRefreshCookie(http, config);
            return Results.NoContent();
        })
        .WithName("Logout")
        .AllowAnonymous();

        // [LOCALSTORAGE] Versión anterior: recibía el refresh token en el cuerpo.
        //group.MapPost("/logout", async (LogoutCommand command, ISender sender) =>
        //{
        //    await sender.Send(command);
        //    return Results.NoContent();
        //})
        //.WithName("Logout")
        //.AllowAnonymous();

        // POST /api/auth/forgot-password — envía por correo el enlace para restablecer la contraseña.
        // Responde SIEMPRE lo mismo, exista o no el correo, para no revelar qué correos están registrados.
        group.MapPost("/forgot-password", async (ForgotPasswordCommand command, ISender sender) =>
        {
            await sender.Send(command);
            return Results.Ok(new
            {
                message = "Si el correo está registrado, recibirás un enlace para restablecer tu contraseña."
            });
        })
        .WithName("ForgotPassword")
        .AllowAnonymous()
        .RequireRateLimiting("AuthRateLimiter");

        // POST /api/auth/reset-password — establece la contraseña nueva con el token del correo.
        // Token inválido, vencido o ya usado => 400 con un mensaje genérico.
        group.MapPost("/reset-password", async (ResetPasswordCommand command, ISender sender) =>
        {
            await sender.Send(command);
            return Results.NoContent();
        })
        .WithName("ResetPassword")
        .AllowAnonymous()
        .RequireRateLimiting("AuthRateLimiter");

        // GET /api/auth/me — requiere JWT válido
        // Opcional: ?roleIds=1&roleIds=2 limita menú y permisos a esos roles (selector del Header).
        group.MapGet("/me", async ([Microsoft.AspNetCore.Mvc.FromQuery] int[]? roleIds, ISender sender) =>
        {
            var result = await sender.Send(new GetMeQuery(roleIds));
            return Results.Ok(result);
        })
        .WithName("Me")
        .RequireAuthorization();
    }

    // ------------------------------------------------------------------
    // Helpers (solo se usan en modo COOKIE)
    // ------------------------------------------------------------------

    /// <summary>
    /// Respuesta de sesión SIN el refresh token (ese viaja solo en la cookie).
    /// Los nombres de propiedad (camelCase al serializar) son los mismos de siempre.
    /// </summary>
    private static object ToSessionResponse(AuthenticateResponse result) => new
    {
        result.AccessToken,
        result.AccessTokenExpiresAt,
        result.User
    };

    private static bool HasClientHeader(HttpContext http) =>
        http.Request.Headers.TryGetValue(ClientHeaderName, out var value)
        && value == ClientHeaderValue;

    private static CookieOptions BuildCookieOptions(IConfiguration config, DateTimeOffset? expires)
    {
        // Auth:RefreshCookie:SameSite (Strict | Lax | None). Por defecto Strict.
        // None solo se usa en desarrollo cuando la web y la API quedan en "sitios" distintos
        // (por ejemplo http://localhost:4200 contra https://localhost:7000).
        var sameSite = Enum.TryParse<SameSiteMode>(config["Auth:RefreshCookie:SameSite"], true, out var parsed)
            ? parsed
            : SameSiteMode.Strict;

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // exige HTTPS (también en SameSite=None)
            SameSite = sameSite,
            Path = RefreshCookiePath, // la cookie solo se envía a /api/auth/*
            Expires = expires,
            IsEssential = true
        };
    }

    private static void SetRefreshCookie(HttpContext http, IConfiguration config, string refreshToken, int days)
    {
        http.Response.Cookies.Append(
            RefreshCookieName,
            refreshToken,
            BuildCookieOptions(config, DateTimeOffset.UtcNow.AddDays(days)));
    }

    private static void DeleteRefreshCookie(HttpContext http, IConfiguration config)
    {
        http.Response.Cookies.Delete(RefreshCookieName, BuildCookieOptions(config, null));
    }
}
