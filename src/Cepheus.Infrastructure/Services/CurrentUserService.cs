using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Utilidades;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Cepheus.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                return int.TryParse(value, out var id) ? id : null;
            }
        }

        public int? RequestedRoleId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.Request.Headers[ActiveRole.HeaderName].FirstOrDefault();
                return int.TryParse(value, out var id) ? id : null;
            }
        }

        public string? Email =>
            _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.Email);

        public string? Login =>
            _httpContextAccessor.HttpContext?.User?
                .FindFirstValue("login");

        public string? FullName
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user is null)
                {
                    return null;
                }

                var firstName = user.FindFirstValue(ClaimTypes.GivenName);
                var lastName = user.FindFirstValue(ClaimTypes.Surname);

                if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
                {
                    return null;
                }

                return $"{firstName} {lastName}".Trim();
            }
        }
    }



}
