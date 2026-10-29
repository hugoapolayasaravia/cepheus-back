using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Catalogos;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Contabilidad
{
    public sealed class ContabilidadCatalogosUnitOfWork
        : IContabilidadCatalogosUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ContabilidadCatalogosUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<TipoAnexo>? _tiposAnexo;
        public IRepository<TipoAnexo> TiposAnexo => _tiposAnexo ??= new Repository<TipoAnexo>(_context);

        private IRepository<TipoCuentaContable>? _tiposCuentaContable;
        public IRepository<TipoCuentaContable> TiposCuentaContable => _tiposCuentaContable ??= new Repository<TipoCuentaContable>(_context);

        private IRepository<TipoAsiento>? _tiposAsiento;
        public IRepository<TipoAsiento> TiposAsiento => _tiposAsiento ??= new Repository<TipoAsiento>(_context);

    }
}