namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Tipo de persona del cliente. Legacy: MClientes.Tipo_cli char(1)
    /// ('E'/'N'), mismo criterio que ProviderType en Logística
    /// (MProveedores.TipoProveedor 'J'/'N').
    /// </summary>
    public enum TipoPersona
    {
        Empresa,
        Natural
    }
}
