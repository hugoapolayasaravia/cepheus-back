using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Mantenimiento.Catalogos;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Mantenimiento
{
    public sealed class MantenimientoCatalogosUnitOfWork
        : IMantenimientoCatalogosUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public MantenimientoCatalogosUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<Inspeccion>? _inspecciones;

        public IRepository<Inspeccion> Inspecciones => _inspecciones ??= new Repository<Inspeccion>(_context);

        private IRepository<Especialidad>? _especialidades;
        public IRepository<Especialidad> Especialidades => _especialidades ??= new Repository<Especialidad>(_context);
        private IRepository<Oportunidad>? _oportunidades;
        public IRepository<Oportunidad> Oportunidades => _oportunidades ??= new Repository<Oportunidad>(_context);
        private IRepository<Prioridad>? _prioridades;
        public IRepository<Prioridad> Prioridades => _prioridades ??= new Repository<Prioridad>(_context);
        private IRepository<TipoOrden>? _tiposOrden;
        public IRepository<TipoOrden> TiposOrden => _tiposOrden ??= new Repository<TipoOrden>(_context);
        private IRepository<Maquina>? _maquinas;
        public IRepository<Maquina> Maquinas => _maquinas ??= new Repository<Maquina>(_context);

    }
}