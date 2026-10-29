using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ValidationException = FluentValidation.ValidationException;

namespace Cepheus.API.Middleware;

/// <summary>
/// Middleware global de manejo de excepciones. Se registra una sola vez en
/// Program.cs, lo más arriba posible del pipeline (antes de UseHttpsRedirection),
/// para capturar cualquier excepción no manejada en el resto de la cadena
/// (endpoint -> MediatR -> ValidationBehavior -> Handler -> EF Core).
///
/// Devuelve siempre application/problem+json (RFC 7807), consistente para
/// cualquier consumidor (Postman, frontend, etc.).
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Las excepciones lanzadas dentro de MethodInfo.Invoke() (usado en ApplySort
        // para el ordenamiento dinámico) llegan envueltas en TargetInvocationException.
        // La desenvolvemos para trabajar siempre con la excepción real.
        exception = UnwrapException(exception);

        var (statusCode, title, detail, errors) = MapExceptionWithDevDetail(exception);

        _logger.Log(
            statusCode >= 500 ? LogLevel.Error : LogLevel.Warning,
            exception,
            "Excepción manejada: {Title} | {Method} {Path}",
            title, context.Request.Method, context.Request.Path);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        // Solo se agrega stack trace para errores inesperados (500+) y solo en Development.
        // Las excepciones de negocio (400/401/404/409) ya tienen un mensaje claro en
        // "detail" y no necesitan trace: son parte del flujo normal, no bugs.
        if (_environment.IsDevelopment() && statusCode >= 500)
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private static Exception UnwrapException(Exception exception)
    {
        while (exception is System.Reflection.TargetInvocationException { InnerException: not null } wrapped)
        {
            exception = wrapped.InnerException!;
        }

        return exception;
    }

    private (int StatusCode, string Title, string Detail, object? Errors) MapException(Exception exception)
        => exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                "Error de validación",
                "Uno o más campos no son válidos.",
                validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            ),

            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                exception.Message,
                null
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "No autorizado",
                exception.Message,
                null
            ),

            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Conflicto de concurrencia",
                "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.",
                null
            ),

            DbUpdateException dbEx => MapDbUpdateException(dbEx),

            JsonException jsonEx => (
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
               BuildJsonErrorDetail(jsonEx),
                //$"El cuerpo de la solicitud contiene datos no válidos: {jsonEx.Message}",
                null
            ),


            Microsoft.AspNetCore.Http.BadHttpRequestException { InnerException: JsonException jsonEx } => (
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                BuildJsonErrorDetail(jsonEx),
                //$"El cuerpo de la solicitud contiene datos no válidos: {jsonEx.Message}",
                null
            ),

                        Microsoft.AspNetCore.Http.BadHttpRequestException => (
                            StatusCodes.Status400BadRequest,
                            "Solicitud inválida",
                            "El cuerpo de la solicitud está vacío o no es un JSON válido. Verifique el body enviado.",
                            null
            ),

            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                "Operación inválida",
                exception.Message,
                null
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrió un error inesperado. Contacte al administrador.",
                null
            )
        };

    /// <summary>
    /// En Development, agrega el tipo y mensaje real de la excepción al detail
    /// cuando cae en el caso genérico 500 — para no tener que adivinar a ciegas
    /// qué pasó. Nunca se usa en Production (podría filtrar detalles internos).
    /// </summary>
    private (int StatusCode, string Title, string Detail, object? Errors) MapExceptionWithDevDetail(Exception exception)
    {
        var (statusCode, title, detail, errors) = MapException(exception);

        if (_environment.IsDevelopment() && statusCode == StatusCodes.Status500InternalServerError)
        {
            detail = $"[{exception.GetType().Name}] {exception.Message}";
        }

        return (statusCode, title, detail, errors);
    }

    private static string BuildJsonErrorDetail(JsonException jsonEx)
    {
        // Path viene como "$.detalles[0].tipo": se muestra sin el "$."
        var campo = string.IsNullOrWhiteSpace(jsonEx.Path)
            ? null
            : jsonEx.Path.TrimStart('$', '.');

        var match = System.Text.RegularExpressions.Regex.Match(
            jsonEx.Message,
            @"could not be converted to ([\w\.\+]+)");

        if (match.Success)
        {
            var typeName = match.Groups[1].Value.TrimEnd('.');

            var type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(typeName, throwOnError: false))
                .FirstOrDefault(t => t is not null);

            if (type is not null && type.IsEnum)
            {
                return $"El valor enviado en '{campo}' no es válido. " +
                       $"Valores permitidos: {string.Join(", ", Enum.GetNames(type))}.";
            }

            if (campo is not null)
            {
                var tipoAmigable = type is not null ? type.Name : typeName;
                return $"El campo '{campo}' tiene un formato no válido (se esperaba {tipoAmigable}).";
            }
        }

        return campo is not null
            ? $"El campo '{campo}' contiene datos no válidos."
            : "El cuerpo de la solicitud contiene datos no válidos.";
    }

    /// <summary>
    /// Traduce errores crudos de SQL Server a mensajes entendibles.
    /// Números de error más comunes:
    ///   2601 / 2627 -> violación de índice único (UNIQUE/PK)
    ///   547         -> violación de FK (referencia inexistente o registro dependiente)
    ///   -2 (timeout)-> timeout de comando/conexión
    /// </summary>
    //private static (int, string, string, object?) MapDbUpdateException(DbUpdateException dbEx)
    //{
    //    if (dbEx.InnerException is SqlException sqlEx)
    //    {
    //        return sqlEx.Number switch
    //        {
    //            2601 or 2627 => (
    //                StatusCodes.Status409Conflict,
    //                "Registro duplicado",
    //                "Ya existe un registro con esos datos (violación de restricción única).",
    //                null
    //            ),
    //            547 => (
    //                StatusCodes.Status409Conflict,
    //                "Violación de integridad referencial",
    //                "La operación viola una relación con otra tabla (registro referenciado o del cual depende otro registro).",
    //                null
    //            ),
    //            -2 => (
    //                StatusCodes.Status504GatewayTimeout,
    //                "Tiempo de espera agotado",
    //                "La operación contra la base de datos tardó demasiado. Intente nuevamente.",
    //                null
    //            ),
    //            _ => (
    //                StatusCodes.Status500InternalServerError,
    //                "Error de base de datos",
    //                "Ocurrió un error al acceder a la base de datos.",
    //                null
    //            )
    //        };
    //    }

    //    return (
    //        StatusCodes.Status500InternalServerError,
    //        "Error de base de datos",
    //        "Ocurrió un error al guardar los cambios.",
    //        null
    //    );
    //}

    private (int, string, string, object?) MapDbUpdateException(DbUpdateException dbEx)
    {
        if (dbEx.InnerException is SqlException sqlEx)
        {
            return sqlEx.Number switch
            {
                2601 or 2627 => (
                    StatusCodes.Status409Conflict,
                    "Registro duplicado",
                    _environment.IsDevelopment()
                        ? sqlEx.Message
                        : "Ya existe un registro con esos datos (violación de restricción única).",
                    null
                ),

                547 => (
                    StatusCodes.Status409Conflict,
                    "Violación de integridad referencial",
                    _environment.IsDevelopment()
                        ? sqlEx.Message
                        : "La operación viola una relación con otra tabla (registro referenciado o del cual depende otro registro).",
                    null
                ),

                -2 => (
                    StatusCodes.Status504GatewayTimeout,
                    "Tiempo de espera agotado",
                    "La operación contra la base de datos tardó demasiado. Intente nuevamente.",
                    null
                ),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Error de base de datos",
                    _environment.IsDevelopment()
                        ? sqlEx.Message
                        : "Ocurrió un error al acceder a la base de datos.",
                    null
                )
            };
        }

        return (
            StatusCodes.Status500InternalServerError,
            "Error de base de datos",
            _environment.IsDevelopment()
                ? dbEx.InnerException?.Message ?? dbEx.Message
                : "Ocurrió un error al guardar los cambios.",
            null
        );
    }
}



public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
