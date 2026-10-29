using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Cepheus.Infrastructure.Authorization
{
    /// <summary>
    /// Reemplaza al provider default de políticas: en vez de exigir que cada política
    /// esté pre-registrada en Program.cs (como .AddPolicy("USERS.CREATE", ...) una por
    /// una), arma la política al vuelo la primera vez que .RequireAuthorization("X.Y")
    /// se usa en un endpoint. Convención: "PROGRAMACODE.PERMISSIONCODE" (ej. "USERS.CREATE"),
    /// coincide con Programa.Code + "." + Permission.Code.
    /// </summary>
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallback;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallback = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var parts = policyName.Split('.', 2);

            if (parts.Length == 2)
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(parts[0], parts[1]))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            // No matchea la convención "X.Y" -> se delega al provider default
            // (políticas estándar como las que arma RequireAuthorization() sin nombre).
            return _fallback.GetPolicyAsync(policyName);
        }
    }

}
