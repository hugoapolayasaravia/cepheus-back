namespace Cepheus.Application.Comun.Interfaces
{
    /// <summary>
    /// Ajustes de la Detracción de Adquiriente. La implementación vive en Infrastructure y se llena
    /// desde la sección "Detraccion" de appsettings.
    /// </summary>
    public interface IDetraccionSettings
    {
        /// <summary>Razón social de la empresa adquiriente (va en la cabecera del archivo de depósito masivo; máx. 35 caracteres).</summary>
        string AdquirienteName { get; }

        /// <summary>RUC de la empresa adquiriente (11 dígitos).</summary>
        string AdquirienteRuc { get; }

        /// <summary>
        /// Código (Comunes.Banco) del Banco de la Nación. De ese banco se toma la cuenta de detracciones
        /// del proveedor en ProveedorCuenta.
        /// </summary>
        string BancoCode { get; }
    }
}
