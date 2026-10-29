namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Origen de la programación de una guía. En el legacy ambas programaciones comparten la columna codigo_pro de
    /// DGuiVentas/RGuiVentas; como sus correlativos son independientes, aquí se guarda también el tipo.
    /// </summary>
    public enum TipoProgramacionGuia
    {
        Concreto = 1,
        Otros = 2,
    }
}
