using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Logistica
{
    public sealed class LogisticaMaestrosUnitOfWork : ILogisticaMaestrosUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public LogisticaMaestrosUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<Proveedor>? _proveedores;

        public IRepository<Proveedor> Proveedores => _proveedores ??= new Repository<Proveedor>(_context);

        
        private IRepository<ProveedorDireccion>? _proveedorDirecciones;

        public IRepository<ProveedorDireccion> ProveedorDirecciones => _proveedorDirecciones ??= new Repository<ProveedorDireccion>(_context);

        
        private IRepository<ProveedorContacto>? _proveedorContactos;

        public IRepository<ProveedorContacto> ProveedorContactos => _proveedorContactos ??= new Repository<ProveedorContacto>(_context);

        
        private IRepository<ProveedorCuenta>? _proveedorCuentas;

        public IRepository<ProveedorCuenta> ProveedorCuentas => _proveedorCuentas ??= new Repository<ProveedorCuenta>(_context);

        
        private IRepository<ProveedorCondicion>? _proveedorCondiciones;

        public IRepository<ProveedorCondicion> ProveedorCondiciones =>  _proveedorCondiciones ??= new Repository<ProveedorCondicion>(_context);

        
        private IRepository<Articulo>? _articulos;

        public IRepository<Articulo> Articulos => _articulos ??= new Repository<Articulo>(_context);

        
        private IRepository<ArticuloProveedor>? _articuloProveedores;

        public IRepository<ArticuloProveedor> ArticuloProveedores => _articuloProveedores ??= new Repository<ArticuloProveedor>(_context);

        
        private IRepository<ArticuloStock>? _stockArticulos;

        public IRepository<ArticuloStock> StockArticulos => _stockArticulos ??= new Repository<ArticuloStock>(_context);

        
        private IRepository<CentroCosto>? _centrosCosto;

        public IRepository<CentroCosto> CentrosCosto =>
            _centrosCosto ??= new Repository<CentroCosto>(_context);

        
        private IRepository<SubCentroCosto>? _subCentrosCosto;

        public IRepository<SubCentroCosto> SubCentrosCosto =>
            _subCentrosCosto ??= new Repository<SubCentroCosto>(_context);

        
        private IRepository<Transportista>? _transportistas;

        public IRepository<Transportista> Transportistas =>
            _transportistas ??= new Repository<Transportista>(_context);

        
        private IRepository<Vehiculo>? _vehiculos;

        public IRepository<Vehiculo> Vehiculos =>
            _vehiculos ??= new Repository<Vehiculo>(_context);

        
        private IRepository<Conductor>? _conductores;

        public IRepository<Conductor> Conductores =>
            _conductores ??= new Repository<Conductor>(_context);

        
        private IRepository<ControlCierre>? _controlCierres;

        public IRepository<ControlCierre> ControlCierres =>
            _controlCierres ??= new Repository<ControlCierre>(_context);


        private IRepository<ArticuloUbicacion>? _articuloUbicaciones;

        public IRepository<ArticuloUbicacion> ArticuloUbicaciones => _articuloUbicaciones ??= new Repository<ArticuloUbicacion>(_context);

        private IRepository<ArticuloPlantaUbicacion>? _articuloPlantaUbicaciones;

        public IRepository<ArticuloPlantaUbicacion> ArticuloPlantaUbicaciones => _articuloPlantaUbicaciones ??= new Repository<ArticuloPlantaUbicacion>(_context);

        private IRepository<SubFamiliaAnexo>? _subFamiliaAnexos;

        public IRepository<SubFamiliaAnexo> SubFamiliaAnexos =>
            _subFamiliaAnexos ??= new Repository<SubFamiliaAnexo>(_context);

        private IRepository<ArticuloAnexo>? _articuloAnexos;
        public IRepository<ArticuloAnexo> ArticuloAnexos => _articuloAnexos ??= new Repository<ArticuloAnexo>(_context);

        private IRepository<ProveedorCuentaContable>? _proveedorCuentasContables;
        public IRepository<ProveedorCuentaContable> ProveedorCuentasContables => _proveedorCuentasContables ??= new Repository<ProveedorCuentaContable>(_context);
    }
}