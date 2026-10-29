using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class ProgramacionConcretoConfiguration : IEntityTypeConfiguration<ProgramacionConcreto>
    {
        public void Configure(EntityTypeBuilder<ProgramacionConcreto> builder)
        {
            builder.ToTable("ProgramacionesConcreto", schema: "facturacion");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.OrdenTrabajoCode).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.ProgramDate).IsRequired().HasColumnType("date");
            builder.Property(x => x.ProgramTime).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.PlantCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ElementoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.TransportObservations).HasMaxLength(50);
            builder.Property(x => x.RequiresLabService).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Status).IsRequired().HasConversion<int>();

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.OrdenTrabajo).WithMany()
                .HasForeignKey(x => x.OrdenTrabajoCode)
                .HasPrincipalKey(o => o.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Planta).WithMany()
                .HasForeignKey(x => x.PlantCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Elemento).WithMany()
                .HasForeignKey(x => x.ElementoCode)
                .HasPrincipalKey(e => e.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Detalles).WithOne(d => d.Programacion)
                .HasForeignKey(d => d.ProgramacionCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.OrdenTrabajoCode);
            builder.HasIndex(x => x.ProgramDate);
            builder.HasIndex(x => x.Status);
        }
    }
}
