using Cepheus.Domain.Mantenimiento.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Mantenimiento - Catalogos

    public DbSet<Inspeccion> Inspecciones => Set<Inspeccion>();
    public DbSet<Especialidad> Especialidades => Set<Especialidad>();
    public DbSet<Oportunidad> Oportunidades => Set<Oportunidad>();
    public DbSet<Prioridad> Prioridades => Set<Prioridad>();
    public DbSet<TipoOrden> TiposOrden => Set<TipoOrden>();
    public DbSet<Maquina> Maquinas => Set<Maquina>();
}