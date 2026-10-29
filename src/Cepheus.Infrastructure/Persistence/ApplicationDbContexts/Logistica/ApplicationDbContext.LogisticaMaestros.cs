using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Logistica - Maestros

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    public DbSet<ProveedorDireccion> ProveedorDirecciones => Set<ProveedorDireccion>();

    public DbSet<ProveedorContacto> ProveedorContactos => Set<ProveedorContacto>();

    public DbSet<ProveedorCuenta> ProveedorCuentas => Set<ProveedorCuenta>();

    public DbSet<ProveedorCondicion> ProveedorCondiciones => Set<ProveedorCondicion>();

    public DbSet<Articulo> Articulos => Set<Articulo>();

    public DbSet<ArticuloProveedor> ArticuloProveedores => Set<ArticuloProveedor>();

    public DbSet<ArticuloStock> StockArticulos => Set<ArticuloStock>();

    public DbSet<CentroCosto> CentrosCosto => Set<CentroCosto>();

    public DbSet<SubCentroCosto> SubCentrosCosto => Set<SubCentroCosto>();

    public DbSet<Transportista> Transportistas => Set<Transportista>();

    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();

    public DbSet<Conductor> Conductores => Set<Conductor>();

    public DbSet<ControlCierre> ControlCierres => Set<ControlCierre>();

    public DbSet<ArticuloUbicacion> ArticuloUbicaciones => Set<ArticuloUbicacion>();

    public DbSet<ArticuloPlantaUbicacion> ArticuloPlantaUbicaciones => Set<ArticuloPlantaUbicacion>();

    public DbSet<SubFamiliaAnexo> SubFamiliaAnexos => Set<SubFamiliaAnexo>();
    public DbSet<ArticuloAnexo> ArticuloAnexos => Set<ArticuloAnexo>();

    public DbSet<ProveedorCuentaContable> ProveedorCuentasContables => Set<ProveedorCuentaContable>();



}