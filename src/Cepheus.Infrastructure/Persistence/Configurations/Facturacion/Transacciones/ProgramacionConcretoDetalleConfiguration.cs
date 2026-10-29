using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class ProgramacionConcretoDetalleConfiguration : IEntityTypeConfiguration<ProgramacionConcretoDetalle>
    {
        public void Configure(EntityTypeBuilder<ProgramacionConcretoDetalle> builder)
        {
            builder.ToTable("ProgramacionesConcretoDetalle", schema: "facturacion");

            builder.HasKey(x => new { x.ProgramacionCode, x.Item });

            builder.Property(x => x.ProgramacionCode).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.OrdenTrabajoCode).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.DeliveredQuantity).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.PumpCode).HasColumnType("char(6)");
            builder.Property(x => x.Support).HasColumnType("tinyint");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Línea de la OT que se programa (la OT es la misma de la cabecera).
            builder.HasOne(x => x.OrdenTrabajoDetalle).WithMany()
                .HasForeignKey(x => new { x.OrdenTrabajoCode, x.Item })
                .HasPrincipalKey(d => new { d.OrdenTrabajoCode, d.Item })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.OrdenTrabajoCode, x.Item });
        }
    }
}
