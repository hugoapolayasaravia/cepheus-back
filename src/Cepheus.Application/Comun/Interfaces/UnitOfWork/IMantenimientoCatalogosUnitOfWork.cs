using Cepheus.Application.Comun.Interfaces;
using Cepheus.Domain.Mantenimiento.Catalogos;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IMantenimientoCatalogosUnitOfWork
    {
        IRepository<Inspeccion> Inspecciones { get; }
        IRepository<Especialidad> Especialidades { get; }
        IRepository<Oportunidad> Oportunidades { get; }
        IRepository<Prioridad> Prioridades { get; }
        IRepository<TipoOrden> TiposOrden { get; }
        IRepository<Maquina> Maquinas { get; }
    }
}