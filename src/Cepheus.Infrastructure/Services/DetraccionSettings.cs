using Cepheus.Application.Comun.Interfaces;

namespace Cepheus.Infrastructure.Services
{
    /// <summary>
    /// Se bindea desde la sección "Detraccion" de appsettings (ver InfrastructureServiceRegistration).
    /// </summary>
    public class DetraccionSettings : IDetraccionSettings
    {
        public string AdquirienteName { get; set; } = string.Empty;
        public string AdquirienteRuc { get; set; } = string.Empty;
        public string BancoCode { get; set; } = string.Empty;
    }
}
