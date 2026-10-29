using Cepheus.Domain.Mantenimiento.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Mantenimiento - Transacciones
    public DbSet<OrdenTrabajo> OrdenesTrabajo => Set<OrdenTrabajo>();
    public DbSet<OTResponsable> OTResponsables => Set<OTResponsable>();
    public DbSet<OTRMaquina> OTRMaquinas => Set<OTRMaquina>();
    public DbSet<OTRMaterial> OTRMateriales => Set<OTRMaterial>();

}