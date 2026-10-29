using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorPensionConfiguration : IEntityTypeConfiguration<TrabajadorPension>
    {
        public void Configure(EntityTypeBuilder<TrabajadorPension> builder)
        {
            builder.ToTable("trabajador_pension", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:1 con Trabajador (igual que el legacy: UNIQUE(trabajador_id))
            builder.HasIndex(x => x.TrabajadorCode).IsUnique();

            builder.HasOne(x => x.TipoAfiliacion).WithMany().HasForeignKey(x => x.TipoAfiliacionCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Afp).WithMany().HasForeignKey(x => x.AfpCode).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.NumeroAfp).HasMaxLength(30);
            builder.HasOne(x => x.RegimenPensionario).WithMany().HasForeignKey(x => x.RegimenPensionarioCode).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.TipoPension).WithMany().HasForeignKey(x => x.TipoPensionCode).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.NumeroCarnetSsp).HasMaxLength(30);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
