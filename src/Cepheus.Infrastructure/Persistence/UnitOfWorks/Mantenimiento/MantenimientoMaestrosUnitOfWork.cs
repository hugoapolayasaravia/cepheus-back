using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Mantenimiento.Maestros;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Mantenimiento
{
    public sealed class MantenimientoMaestrosUnitOfWork
        : IMantenimientoMaestrosUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public MantenimientoMaestrosUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<Equipo>? _equipos;
        public IRepository<Equipo> Equipos => _equipos ??= new Repository<Equipo>(_context);

        private IRepository<CentroEjecutor>? _centrosEjecutores;
        public IRepository<CentroEjecutor> CentrosEjecutores => _centrosEjecutores ??= new Repository<CentroEjecutor>(_context);
        private IRepository<SubCentroEjecutor>? _subCentrosEjecutores;
        public IRepository<SubCentroEjecutor> SubCentrosEjecutores => _subCentrosEjecutores ??= new Repository<SubCentroEjecutor>(_context);
        private IRepository<VerboActividad>? _verbosActividad;
        public IRepository<VerboActividad> VerbosActividad => _verbosActividad ??= new Repository<VerboActividad>(_context);
        private IRepository<ObjetoActividad>? _objetosActividad;
        public IRepository<ObjetoActividad> ObjetosActividad => _objetosActividad ??= new Repository<ObjetoActividad>(_context);
        private IRepository<Actividad>? _actividades;
        public IRepository<Actividad> Actividades => _actividades ??= new Repository<Actividad>(_context);

    }
}