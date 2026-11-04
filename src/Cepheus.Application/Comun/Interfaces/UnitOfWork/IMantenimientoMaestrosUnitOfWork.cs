using Cepheus.Domain.Mantenimiento.Maestros;
using Cepheus.Domain.Mantenimiento.Transacciones;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IMantenimientoMaestrosUnitOfWork
    {

        IRepository<Equipo> Equipos { get; }
        IRepository<CentroEjecutor> CentrosEjecutores { get; }
        IRepository<SubCentroEjecutor> SubCentrosEjecutores { get; }
        IRepository<VerboActividad> VerbosActividad { get; }
        IRepository<ObjetoActividad> ObjetosActividad { get; }
        IRepository<Actividad> Actividades { get; }
        IRepository<ActividadMantenimiento> ActividadesMantenimiento { get; }


    }
}