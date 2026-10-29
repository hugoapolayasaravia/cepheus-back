namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IRrhhUnitOfWork
    {
        IRrhhCatalogosUnitOfWork Catalogos { get; }

        IRrhhMaestrosUnitOfWork Maestros { get; }

        IRrhhTransaccionesUnitOfWork Transacciones { get; }
    }
}