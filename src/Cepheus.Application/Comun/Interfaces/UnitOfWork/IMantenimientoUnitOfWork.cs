namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IMantenimientoUnitOfWork
    {
        IMantenimientoCatalogosUnitOfWork Catalogos { get; }

        IMantenimientoMaestrosUnitOfWork Maestros { get; }

        IMantenimientoTransaccionesUnitOfWork Transacciones { get; }
    }
}