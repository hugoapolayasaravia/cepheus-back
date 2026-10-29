using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Comunes
{
    public sealed class ComunesUnitOfWork : IComunesUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ComunesUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<Planta>? _plantas;
        public IRepository<Planta> Plantas => _plantas ??= new Repository<Planta>(_context);

        private IRepository<Moneda>? _monedas;
        public IRepository<Moneda> Monedas => _monedas ??= new Repository<Moneda>(_context);

        private IRepository<TipoDocumento>? _tiposDocumento;
        public IRepository<TipoDocumento> TiposDocumento => _tiposDocumento ??= new Repository<TipoDocumento>(_context);

        private IRepository<ComprobantePago>? _comprobantesPago;
        public IRepository<ComprobantePago> ComprobantesPago => _comprobantesPago ??= new Repository<ComprobantePago>(_context);

        private IRepository<Ubigeo>? _ubigeos;
        public IRepository<Ubigeo> Ubigeos => _ubigeos ??= new Repository<Ubigeo>(_context);

        private IRepository<TipoCambio>? _tiposCambio;
        public IRepository<TipoCambio> TiposCambio => _tiposCambio ??= new Repository<TipoCambio>(_context);

        private IRepository<ControlVentas>? _controlesVentas;
        public IRepository<ControlVentas> ControlesVentas => _controlesVentas ??= new Repository<ControlVentas>(_context);

        private IRepository<MotivoDevolucion>? _motivosDevolucion;
        public IRepository<MotivoDevolucion> MotivosDevolucion => _motivosDevolucion ??= new Repository<MotivoDevolucion>(_context);

        private IRepository<Banco>? _bancos;
        public IRepository<Banco> Bancos => _bancos ??= new Repository<Banco>(_context);

        private IRepository<Negocio>? _negocios;
        public IRepository<Negocio> Negocios => _negocios ??= new Repository<Negocio>(_context);

        private IRepository<PeriodoIgv>? _periodosIgv;
        public IRepository<PeriodoIgv> PeriodosIgv => _periodosIgv ??= new Repository<PeriodoIgv>(_context);
    }
}
