using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Contabilidad
{
    public sealed class ContabilidadTransaccionesUnitOfWork
        : IContabilidadTransaccionesUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ContabilidadTransaccionesUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<DocumentoCompra>? _documentosCompra;
        public IRepository<DocumentoCompra> DocumentosCompra => _documentosCompra ??= new Repository<DocumentoCompra>(_context);

        private IRepository<DocumentoCompraDetalle>? _documentoCompraDetalles;
        public IRepository<DocumentoCompraDetalle> DocumentoCompraDetalles => _documentoCompraDetalles ??= new Repository<DocumentoCompraDetalle>(_context);

        private IRepository<AsientoContable>? _asientosContables;
        public IRepository<AsientoContable> AsientosContables => _asientosContables ??= new Repository<AsientoContable>(_context);

        private IRepository<AsientoContableDetalle>? _asientoContableDetalles;
        public IRepository<AsientoContableDetalle> AsientoContableDetalles => _asientoContableDetalles ??= new Repository<AsientoContableDetalle>(_context);
        
        private IRepository<CajaChica>? _cajasChica;
        public IRepository<CajaChica> CajasChica => _cajasChica ??= new Repository<CajaChica>(_context);

        private IRepository<CajaChicaDetalle>? _cajasChicaDetalle;
        public IRepository<CajaChicaDetalle> CajasChicaDetalle => _cajasChicaDetalle ??= new Repository<CajaChicaDetalle>(_context);

        private IRepository<Detraccion>? _detracciones;
        public IRepository<Detraccion> Detracciones => _detracciones ??= new Repository<Detraccion>(_context);

        private IRepository<DetraccionItem>? _detraccionItems;
        public IRepository<DetraccionItem> DetraccionItems => _detraccionItems ??= new Repository<DetraccionItem>(_context);

        private IRepository<DetraccionDocumento>? _detraccionDocumentos;
        public IRepository<DetraccionDocumento> DetraccionDocumentos => _detraccionDocumentos ??= new Repository<DetraccionDocumento>(_context);

    }
}