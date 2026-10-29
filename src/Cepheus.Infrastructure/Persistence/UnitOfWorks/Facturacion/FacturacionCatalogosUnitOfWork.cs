using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Facturacion
{
    public sealed class FacturacionCatalogosUnitOfWork
        : IFacturacionCatalogosUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public FacturacionCatalogosUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<TipoCliente>? _tiposCliente;
        public IRepository<TipoCliente> TiposCliente => _tiposCliente ??= new Repository<TipoCliente>(_context);

        private IRepository<ClasificacionCliente>? _clasificacionesCliente;
        public IRepository<ClasificacionCliente> ClasificacionesCliente => _clasificacionesCliente ??= new Repository<ClasificacionCliente>(_context);

        private IRepository<SegmentoVentas>? _segmentosVentas;
        public IRepository<SegmentoVentas> SegmentosVentas => _segmentosVentas ??= new Repository<SegmentoVentas>(_context);

        private IRepository<AnalisisVenta>? _analisisVentas;
        public IRepository<AnalisisVenta> AnalisisVentas => _analisisVentas ??= new Repository<AnalisisVenta>(_context);

        private IRepository<TipoValorizacion>? _tiposValorizacion;
        public IRepository<TipoValorizacion> TiposValorizacion => _tiposValorizacion ??= new Repository<TipoValorizacion>(_context);

        private IRepository<FormaPagoVenta>? _formasPagoVenta;

        public IRepository<FormaPagoVenta> FormasPagoVenta => _formasPagoVenta ??= new Repository<FormaPagoVenta>(_context);

        private IRepository<CategoriaProducto>? _categoriasProducto;
        public IRepository<CategoriaProducto> CategoriasProducto => _categoriasProducto ??= new Repository<CategoriaProducto>(_context);

        private IRepository<TipoBien>? _tiposBien;
        public IRepository<TipoBien> TiposBien => _tiposBien ??= new Repository<TipoBien>(_context);

        private IRepository<TipoOperacion>? _tiposOperacion;
        public IRepository<TipoOperacion> TiposOperacion => _tiposOperacion ??= new Repository<TipoOperacion>(_context);

        private IRepository<AtributoConcreto>? _atributosConcreto;
        public IRepository<AtributoConcreto> AtributosConcreto => _atributosConcreto ??= new Repository<AtributoConcreto>(_context);

        private IRepository<TipoProducto>? _tiposProducto;
        public IRepository<TipoProducto> TiposProducto => _tiposProducto ??= new Repository<TipoProducto>(_context);
       
        private IRepository<UnidadMedidaVenta>? _unidadesMedidaVenta;
        public IRepository<UnidadMedidaVenta> UnidadesMedidaVenta => _unidadesMedidaVenta ??= new Repository<UnidadMedidaVenta>(_context);

        private IRepository<ListaPrecio>? _listasPrecio;
        public IRepository<ListaPrecio> ListasPrecio => _listasPrecio ??= new Repository<ListaPrecio>(_context);

        private IRepository<StockProducto>? _stockProductos;
        public IRepository<StockProducto> StockProductos => _stockProductos ??= new Repository<StockProducto>(_context);

        private IRepository<Flete>? _fletes;
        public IRepository<Flete> Fletes => _fletes ??= new Repository<Flete>(_context);

        private IRepository<AlturaLosa>? _alturasLosa;
        public IRepository<AlturaLosa> AlturasLosa => _alturasLosa ??= new Repository<AlturaLosa>(_context);

        private IRepository<PrecioProducto>? _preciosProducto;
        public IRepository<PrecioProducto> PreciosProducto => _preciosProducto ??= new Repository<PrecioProducto>(_context);

        private IRepository<NotaCotizacionPlantilla>? _notasCotizacionPlantilla;
        public IRepository<NotaCotizacionPlantilla> NotasCotizacionPlantilla => _notasCotizacionPlantilla ??= new Repository<NotaCotizacionPlantilla>(_context);

        private IRepository<ResponsableObra>? _responsablesObra;
        public IRepository<ResponsableObra> ResponsablesObra => _responsablesObra ??= new Repository<ResponsableObra>(_context);

        private IRepository<TipoHojaCorte>? _tiposHojaCorte;
        public IRepository<TipoHojaCorte> TiposHojaCorte => _tiposHojaCorte ??= new Repository<TipoHojaCorte>(_context);

        private IRepository<MotivoSolicitudNotaCredito>? _motivosSolicitudNotaCredito;
        public IRepository<MotivoSolicitudNotaCredito> MotivosSolicitudNotaCredito => _motivosSolicitudNotaCredito ??= new Repository<MotivoSolicitudNotaCredito>(_context);

        private IRepository<ElementoConcreto>? _elementosConcreto;
        public IRepository<ElementoConcreto> ElementosConcreto => _elementosConcreto ??= new Repository<ElementoConcreto>(_context);
    }
}