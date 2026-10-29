using Cepheus.Domain.Mantenimiento.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Mantenimiento.Transacciones
{
    public class OTRMaterialConfiguration : IEntityTypeConfiguration<OTRMaterial>
    {
        public void Configure(EntityTypeBuilder<OTRMaterial> builder)
        {
            builder.ToTable("OTRMateriales", schema: "mantenimiento");

            builder.HasKey(x => new { x.PlantaCode, x.OrdenTrabajoCode, x.FechaProceso, x.ArticuloCode });

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                 .HasColumnType("char(2)");

            builder.Property(x => x.OrdenTrabajoCode)
                .IsRequired()
                 .HasColumnType("char(6)");

            builder.HasOne(x => x.OrdenTrabajo)
                .WithMany()
                .HasForeignKey(x => new { x.PlantaCode, x.OrdenTrabajoCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ArticuloCode).IsRequired().HasMaxLength(7);
            builder.HasOne(x => x.Articulo)
                .WithMany()
                .HasForeignKey(x => x.ArticuloCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.FechaProceso).IsRequired();

            builder.Property(x => x.Cantidad).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);
            builder.Property(x => x.CostoUnitario).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);
            builder.Property(x => x.CostoTotal).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);

            builder.Property(x => x.EstadoCode).HasMaxLength(2);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
