using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Mantenimiento
{
    public sealed class MantenimientoUnitOfWork : IMantenimientoUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IMantenimientoCatalogosUnitOfWork? _catalogos;
        private IMantenimientoMaestrosUnitOfWork? _maestros;
        private IMantenimientoTransaccionesUnitOfWork? _transacciones;

        public MantenimientoUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IMantenimientoCatalogosUnitOfWork Catalogos =>
            _catalogos ??= new MantenimientoCatalogosUnitOfWork(_context);

        public IMantenimientoMaestrosUnitOfWork Maestros =>
            _maestros ??= new MantenimientoMaestrosUnitOfWork(_context);

        public IMantenimientoTransaccionesUnitOfWork Transacciones =>
            _transacciones ??= new MantenimientoTransaccionesUnitOfWork(_context);



    }
}