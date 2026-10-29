using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Facturacion
{
    public sealed class FacturacionUnitOfWork : IFacturacionUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IFacturacionCatalogosUnitOfWork? _catalogos;
        private IFacturacionMaestrosUnitOfWork? _maestros;
        private IFacturacionTransaccionesUnitOfWork? _transacciones;

        public FacturacionUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IFacturacionCatalogosUnitOfWork Catalogos =>
            _catalogos ??= new FacturacionCatalogosUnitOfWork(_context);

        public IFacturacionMaestrosUnitOfWork Maestros =>
            _maestros ??= new FacturacionMaestrosUnitOfWork(_context);

        public IFacturacionTransaccionesUnitOfWork Transacciones =>
            _transacciones ??= new FacturacionTransaccionesUnitOfWork(_context);



    }
}