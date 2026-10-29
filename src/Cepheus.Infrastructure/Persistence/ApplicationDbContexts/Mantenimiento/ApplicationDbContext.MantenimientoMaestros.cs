using Cepheus.Domain.Mantenimiento.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Mantenimiento - Maestros
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<CentroEjecutor> CentrosEjecutores => Set<CentroEjecutor>();
    public DbSet<SubCentroEjecutor> SubCentrosEjecutores => Set<SubCentroEjecutor>();
    public DbSet<VerboActividad> VerbosActividad => Set<VerboActividad>();
    public DbSet<ObjetoActividad> ObjetosActividad => Set<ObjetoActividad>();
    public DbSet<Actividad> Actividades => Set<Actividad>();

}