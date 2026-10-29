using Cepheus.Application.Comun.Interfaces;

namespace Cepheus.Infrastructure.Services
{
    /// <summary>
    /// Se bindea desde la sección "Reports" de appsettings (ver InfrastructureServiceRegistration).
    /// </summary>
    public class ReportSettings : IReportSettings
    {
        public string CompanyName { get; set; } = "Cepheus";
    }
}
