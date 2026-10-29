using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Comunes

    public DbSet<Planta> Plantas => Set<Planta>();

    public DbSet<Moneda> Monedas => Set<Moneda>();

    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();

    public DbSet<ComprobantePago> ComprobantesPago => Set<ComprobantePago>();

    public DbSet<Ubigeo> Ubigeos => Set<Ubigeo>();

    public DbSet<TipoCambio> TiposCambio => Set<TipoCambio>();

    public DbSet<ControlVentas> ControlesVentas => Set<ControlVentas>();

    public DbSet<MotivoDevolucion> MotivosDevolucion => Set<MotivoDevolucion>();

    public DbSet<Banco> Bancos => Set<Banco>();
    public DbSet<Negocio> Negocios => Set<Negocio>();

    public DbSet<PeriodoIgv> PeriodosIgv => Set<PeriodoIgv>();
}