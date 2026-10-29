using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Logistica - Catalogos

    public DbSet<Familia> Familias => Set<Familia>();

    public DbSet<SubFamilia> SubFamilias => Set<SubFamilia>();

    public DbSet<UnidadMedida> UnidadesMedida => Set<UnidadMedida>();

    public DbSet<TipoCompra> TiposCompra => Set<TipoCompra>();

    public DbSet<NotaCompra> NotasCompra => Set<NotaCompra>();

    public DbSet<LugarEnvio> LugaresEnvio => Set<LugarEnvio>();

    public DbSet<Comprador> Compradores => Set<Comprador>();

    public DbSet<Tramite> Tramites => Set<Tramite>();

    public DbSet<TipoPedido> TiposPedido => Set<TipoPedido>();

    public DbSet<UnidadNegocio> UnidadesNegocio => Set<UnidadNegocio>();

    public DbSet<TipoVale> TiposVale => Set<TipoVale>();

    public DbSet<TipoArticulo> TiposArticulo => Set<TipoArticulo>();

    public DbSet<PlanArticulo> PlanesArticulo => Set<PlanArticulo>();

    public DbSet<FormaPago> FormasPago => Set<FormaPago>();
    public DbSet<Nivel> Niveles => Set<Nivel>();
    public DbSet<TipoTransaccion> TiposTransaccion => Set<TipoTransaccion>();
    public DbSet<RangoAprobacion> RangosAprobacion => Set<RangoAprobacion>();
    public DbSet<AprobadorAsignado> AprobadoresAsignados => Set<AprobadorAsignado>();

    public DbSet<MotivoDevolucionArticulo> MotivoDevolucionArticulos => Set<MotivoDevolucionArticulo>();

    public DbSet<TipoValeArticulo> TiposValeArticulo => Set<TipoValeArticulo>();

    public DbSet<UsuarioPlanta> UsuariosPlanta => Set<UsuarioPlanta>();
}