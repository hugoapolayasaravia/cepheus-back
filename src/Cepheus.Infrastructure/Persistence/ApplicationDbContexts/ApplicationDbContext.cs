using Cepheus.Application.Comun.Interfaces;
using Cepheus.Domain.Comun;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts
{

    public partial class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        DatabaseFacade IApplicationDbContext.Database => base.Database;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica todas las IEntityTypeConfiguration<> del ensamblado
            // (Configurations/Administracion/*, Configurations/Logistica/* a futuro)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Auditoría automática: setea CreatedAt/CreatedBy en entidades nuevas
        /// y UpdatedAt/UpdatedBy en entidades modificadas, para toda entidad
        /// que implemente IAuditableEntity. Los handlers NO deben setear
        /// estos campos manualmente.
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;
            var currentUserName = BuildCurrentUserAuditLabel();

            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = utcNow;
                        entry.Entity.CreatedBy = currentUserName;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = utcNow;
                        entry.Entity.UpdatedBy = currentUserName;
                        // Nunca se debe pisar CreatedAt/CreatedBy en un Update
                        entry.Property(x => x.CreatedAt).IsModified = false;
                        entry.Property(x => x.CreatedBy).IsModified = false;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// "Nombre Apellido (email)" — el email desambigua entre dos usuarios
        /// con el mismo nombre y apellido en el registro de auditoría.
        /// "System" si no hay usuario autenticado (ej. bootstrap de Register).
        /// </summary>
        private string BuildCurrentUserAuditLabel()
        {
            var fullName = _currentUserService.FullName;
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "System";
            }

            var email = _currentUserService.Email;
            return string.IsNullOrWhiteSpace(email) ? fullName : $"{fullName} ({email})";
        }
    }






}
