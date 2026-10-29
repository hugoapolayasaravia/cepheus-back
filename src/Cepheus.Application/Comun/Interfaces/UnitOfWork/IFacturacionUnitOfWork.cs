namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IFacturacionUnitOfWork
    {
        IFacturacionCatalogosUnitOfWork Catalogos { get; }

        IFacturacionMaestrosUnitOfWork Maestros { get; }

        IFacturacionTransaccionesUnitOfWork Transacciones { get; }
    }
}