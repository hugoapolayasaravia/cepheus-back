using Cepheus.API.Authorization;
using Cepheus.API.Extensions.Endpoints.Administracion;
using Cepheus.API.Extensions.Endpoints.Comunes;
using Cepheus.API.Extensions.Endpoints.Contabilidad;
using Cepheus.API.Extensions.Endpoints.Facturacion;
using Cepheus.API.Extensions.Endpoints.Logistica;
using Cepheus.API.Extensions.Endpoints.Mantenimiento;
using Cepheus.API.Extensions.Endpoints.Rrhh;
using Cepheus.API.Middleware;
using Cepheus.Infrastructure;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Seed;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "CepheusCorsPolicy";
const string AuthRateLimiterPolicyName = "AuthRateLimiter";

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Cepheus.Application.AssemblyReference).Assembly));

builder.Services.AddValidatorsFromAssembly(
    typeof(Cepheus.Application.AssemblyReference).Assembly);

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(Cepheus.Application.Comun.Behaviors.ValidationBehavior<,>));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        var secretKey = jwt["SecretKey"];

        // Falla rápido y ruidoso en vez de arrancar silenciosamente con una
        // clave vacía o un placeholder conocido.
        if (string.IsNullOrWhiteSpace(secretKey) ||
            secretKey.Contains("CAMBIAR", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Jwt:SecretKey no está configurado. En Development, corré " +
                "'dotnet user-secrets set \"Jwt:SecretKey\" \"<clave-real>\"' desde Cepheus.API. " +
                "En Production, configurá la variable de entorno Jwt__SecretKey.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// Personaliza la respuesta cuando el usuario está autenticado
// pero no tiene los permisos requeridos para el endpoint.
builder.Services.AddSingleton<
    IAuthorizationMiddlewareResultHandler,
    CustomAuthorizationMiddlewareResultHandler>();

// CORS: los orígenes permitidos vienen de appsettings
// (Cors:AllowedOrigins), no están hardcodeados acá.
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

//builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
//{
//    options.SerializerOptions.UnmappedMemberHandling =
//        System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow;
//});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.UnmappedMemberHandling =
        System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow;

    options.SerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Rate limiting: solo en login/register.
// 5 intentos cada 1 minuto por IP.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/problem+json";

        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status429TooManyRequests,
            Title = "Demasiados intentos",
            Detail = "Se superó el límite de intentos permitidos. Intente nuevamente en un minuto.",
            Instance = context.HttpContext.Request.Path,
            Type = "https://httpstatuses.com/429"
        };

        await context.HttpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);
    };

    options.AddPolicy(
        AuthRateLimiterPolicyName,
        httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey:
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown",

                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));
});

var app = builder.Build();

// Seed del árbol de Administración:
// Modulo "ADMIN" -> Submodulo "SEG" ->
// USERS / ROLES / MODULOS / SUBMODULOS / PROGRAMAS / PERMISSIONS.
// También sincroniza el rol Administrador con todo lo sembrado.
// Es idempotente.
using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await AdministracionSeeder.SeedAsync(dbContext);
    await ComunesSeeder.SeedAsync(dbContext);
    await LogisticaSeeder.SeedAsync(dbContext);
}

app.UseExceptionHandling();
app.UseSecurityHeaders();

// HSTS solo fuera de Development.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

// Administracion
app.MapAdministracionEndpoints();

// Comunes
app.MapComunesEndpoints();

// Logistica
app.MapLogisticaCatalogosEndpoints();
app.MapLogisticaMaestrosEndpoints();
app.MapLogisticaTransaccionesEndpoints();
app.MapLogisticaReportesEndpoints();

// Mantenimiento
app.MapMantenimientoCatalogosEndpoints();
app.MapMantenimientoMaestrosEndpoints();
app.MapMantenimientoTransaccionesEndpoints();
app.MapMantenimientoReportesEndpoints();

// Recursos Humanos
app.MapRrhhCatalogosEndpoints();
app.MapRrhhMaestrosEndpoints();
app.MapRrhhTransaccionesEndpoints();

// Facturacion
app.MapFacturacionCatalogosEndpoints();
app.MapFacturacionMaestrosEndpoints();
app.MapFacturacionTransaccionesEndpoints();


// Contabilidad
app.MapContabilidadCatalogosEndpoints();
app.MapContabilidadMaestrosEndpoints();
app.MapContabilidadTransaccionesEndpoints();

app.Run();
