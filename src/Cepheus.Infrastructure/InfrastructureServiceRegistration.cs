using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;
using Cepheus.Infrastructure.Authorization;
using Cepheus.Infrastructure.Identity;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.UnitOfWorks;
using Cepheus.Infrastructure.Services;
using Cepheus.Infrastructure.Services.StockArticulos;
using Cepheus.Infrastructure.Services.Transferencias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JwtTokenService = Cepheus.Infrastructure.Identity.JwtTokenService;

namespace Cepheus.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddScoped<IPasswordHasher, PasswordHasherService>();

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IUserInvitationService, UserInvitationService>();
            services.Configure<ReportSettings>(configuration.GetSection("Reports"));
            services.AddScoped<IReportSettings>(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ReportSettings>>().Value);

            services.Configure<DetraccionSettings>(configuration.GetSection("Detraccion"));
            services.AddScoped<IDetraccionSettings>(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DetraccionSettings>>().Value);

            services.Configure<PasswordResetSettings>(configuration.GetSection("PasswordReset"));
            services.AddScoped<IPasswordResetSettings>(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PasswordResetSettings>>().Value);

            // Autorización dinámica por Permission/permisos_roles (ver Cepheus.Infrastructure.Authorization)
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddScoped<IArticuloStockMovementService, ArticuloStockMovementService>();

            // Revaluadores del cierre mensual: uno por tipo de documento de salida.
            services.AddScoped<ICierreRevaluador, OrdenServicioSalidaRevaluador>();
            services.AddScoped<ICierreRevaluador, ValeSalidaRevaluador>();
            services.AddScoped<ICierreRevaluador, AjusteInventarioSalidaRevaluador>();

            // Transferencias entre almacenes: el costo final del origen se lleva al ingreso del destino.
            services.AddScoped<TransferenciaIngresoRevaluador>();
            services.AddScoped<ICierreRevaluador>(sp => sp.GetRequiredService<TransferenciaIngresoRevaluador>());
            services.AddScoped<IPropagadorRevaluacion>(sp => sp.GetRequiredService<TransferenciaIngresoRevaluador>());
            services.AddSingleton<ITransferenciaSettings, TransferenciaSettings>();

            return services;
        }
    }
}
