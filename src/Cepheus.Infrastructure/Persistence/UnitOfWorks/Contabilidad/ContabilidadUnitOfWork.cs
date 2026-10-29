using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Contabilidad
{
    public sealed class ContabilidadUnitOfWork : IContabilidadUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IContabilidadCatalogosUnitOfWork? _catalogos;
        private IContabilidadMaestrosUnitOfWork? _maestros;
        private IContabilidadTransaccionesUnitOfWork? _transacciones;

        public ContabilidadUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IContabilidadCatalogosUnitOfWork Catalogos =>
            _catalogos ??= new ContabilidadCatalogosUnitOfWork(_context);

        public IContabilidadMaestrosUnitOfWork Maestros =>
            _maestros ??= new ContabilidadMaestrosUnitOfWork(_context);

        public IContabilidadTransaccionesUnitOfWork Transacciones =>
            _transacciones ??= new ContabilidadTransaccionesUnitOfWork(_context);



    }
}