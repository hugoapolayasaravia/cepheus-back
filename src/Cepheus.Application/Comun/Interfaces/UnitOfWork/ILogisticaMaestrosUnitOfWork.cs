using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface ILogisticaMaestrosUnitOfWork
    {
        IRepository<Proveedor> Proveedores { get; }
        IRepository<ProveedorDireccion> ProveedorDirecciones { get; }
        IRepository<ProveedorContacto> ProveedorContactos { get; }
        IRepository<ProveedorCuenta> ProveedorCuentas { get; }
        IRepository<ProveedorCondicion> ProveedorCondiciones { get; }

        IRepository<Articulo> Articulos { get; }
        IRepository<ArticuloProveedor> ArticuloProveedores { get; }
        IRepository<ArticuloStock> StockArticulos { get; }

        IRepository<CentroCosto> CentrosCosto { get; }
        IRepository<SubCentroCosto> SubCentrosCosto { get; }

        IRepository<Transportista> Transportistas { get; }
        IRepository<Vehiculo> Vehiculos { get; }
        IRepository<Conductor> Conductores { get; }

        IRepository<ControlCierre> ControlCierres { get; }

        IRepository<ArticuloUbicacion> ArticuloUbicaciones { get; }

        IRepository<ArticuloPlantaUbicacion> ArticuloPlantaUbicaciones { get; }

        IRepository<SubFamiliaAnexo> SubFamiliaAnexos { get; }
        IRepository<ArticuloAnexo> ArticuloAnexos { get; }

        IRepository<ProveedorCuentaContable> ProveedorCuentasContables { get; }

    }
}
