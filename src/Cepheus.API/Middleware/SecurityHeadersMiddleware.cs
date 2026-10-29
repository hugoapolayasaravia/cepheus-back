namespace Cepheus.API.Middleware
{
    /// <summary>
    /// Agrega headers de seguridad a TODA respuesta. No reemplaza HSTS (eso lo
    /// maneja app.UseHsts(), nativo de ASP.NET Core, solo activo en Production)
    /// — este middleware cubre lo que HSTS no cubre.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Response.Headers;

            // Evita que el navegador "adivine" el Content-Type (protege contra
            // ataques que suben un .txt disfrazado de .html/.js, por ejemplo).
            headers["X-Content-Type-Options"] = "nosniff";

            // Prohíbe que esta API se embeba en un <iframe> de otro sitio
            // (mitiga clickjacking). "DENY" = ni siquiera el propio dominio.
            headers["X-Frame-Options"] = "DENY";

            // No envía el header Referer al navegar hacia afuera de este origen,
            // y lo recorta al mínimo incluso navegando dentro del mismo origen.
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Deshabilita APIs del navegador que esta API (backend puro, sin UI)
            // no tiene ningún motivo para usar.
            headers["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";

            await _next(context);
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
            => app.UseMiddleware<SecurityHeadersMiddleware>();
    }

}
