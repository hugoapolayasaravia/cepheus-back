using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{

    public DbSet<TransportistaVenta> FacturacionTransportistas => Set<TransportistaVenta>();

    public DbSet<ChoferVenta> FacturacionChoferes => Set<ChoferVenta>();

    public DbSet<VehiculoVenta> FacturacionVehiculos => Set<VehiculoVenta>();

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Obra> Obras => Set<Obra>();

    public DbSet<Vendedor> Vendedores => Set<Vendedor>();
    public DbSet<Cobrador> Cobradores => Set<Cobrador>();
    public DbSet<Producto> Productos => Set<Producto>();

    public DbSet<Tecnico> Tecnicos => Set<Tecnico>();
    public DbSet<UsuarioSolicitudNotaCredito> UsuariosSolicitudNotaCredito => Set<UsuarioSolicitudNotaCredito>();

}