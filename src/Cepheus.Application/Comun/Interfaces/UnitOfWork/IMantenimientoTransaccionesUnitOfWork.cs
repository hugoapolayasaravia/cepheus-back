using Cepheus.Domain.Mantenimiento.Transacciones;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IMantenimientoTransaccionesUnitOfWork
    {

        IRepository<OrdenTrabajo> OrdenesTrabajo { get; }
        IRepository<OTResponsable> OTResponsables { get; }
        IRepository<OTRMaquina> OTRMaquinas { get; }
        IRepository<OTRMaterial> OTRMateriales { get; }


    }
}