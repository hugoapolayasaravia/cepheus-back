using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.UnitOfWorks.Rrhh;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks
{
    public sealed class RrhhUnitOfWork : IRrhhUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IRrhhCatalogosUnitOfWork? _catalogos;
        private IRrhhMaestrosUnitOfWork? _maestros;
        private IRrhhTransaccionesUnitOfWork? _transacciones;

        public RrhhUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRrhhCatalogosUnitOfWork Catalogos =>
            _catalogos ??= new RrhhCatalogosUnitOfWork(_context);

        public IRrhhMaestrosUnitOfWork Maestros =>
            _maestros ??= new RrhhMaestrosUnitOfWork(_context);

        public IRrhhTransaccionesUnitOfWork Transacciones =>
            _transacciones ??= new RrhhTransaccionesUnitOfWork(_context);



    }
}