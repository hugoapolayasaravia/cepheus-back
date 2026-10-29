namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IContabilidadUnitOfWork
    {
        IContabilidadCatalogosUnitOfWork Catalogos { get; }

        IContabilidadMaestrosUnitOfWork Maestros { get; }

        IContabilidadTransaccionesUnitOfWork Transacciones { get; }
    }
}