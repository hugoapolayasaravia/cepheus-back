namespace Cepheus.Application.Features.Administracion.Users.Common
{
    public class AuthenticateResponse
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTime AccessTokenExpiresAt { get; set; }
        public UserResponse User { get; set; } = default!;
    }
}
