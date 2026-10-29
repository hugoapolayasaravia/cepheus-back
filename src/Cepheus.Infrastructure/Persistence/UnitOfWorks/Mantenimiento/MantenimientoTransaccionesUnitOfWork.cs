using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Mantenimiento.Transacciones;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Mantenimiento
{
    public sealed class MantenimientoTransaccionesUnitOfWork
        : IMantenimientoTransaccionesUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public MantenimientoTransaccionesUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<OrdenTrabajo>? _ordenesTrabajo;
        public IRepository<OrdenTrabajo> OrdenesTrabajo => _ordenesTrabajo ??= new Repository<OrdenTrabajo>(_context);
        private IRepository<OTResponsable>? _otResponsables;
        public IRepository<OTResponsable> OTResponsables => _otResponsables ??= new Repository<OTResponsable>(_context);
        private IRepository<OTRMaquina>? _otrMaquinas;
        public IRepository<OTRMaquina> OTRMaquinas => _otrMaquinas ??= new Repository<OTRMaquina>(_context);
        private IRepository<OTRMaterial>? _otrMateriales;
        public IRepository<OTRMaterial> OTRMateriales => _otrMateriales ??= new Repository<OTRMaterial>(_context);

    }
}