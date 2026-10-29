namespace Cepheus.Application.Comun.Interfaces
{
    /// <summary>
    /// Ajustes de los reportes. La implementación vive en Infrastructure y se llena
    /// desde la sección "Reports" de appsettings.
    /// </summary>
    public interface IReportSettings
    {
        /// <summary>Nombre de la empresa que aparece arriba a la izquierda en cada hoja del PDF.</summary>
        string CompanyName { get; }
    }
}
