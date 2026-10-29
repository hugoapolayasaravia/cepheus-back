using Cepheus.Domain.Mantenimiento.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Mantenimiento.Maestros
{
    public class PlanMantenimientoPreventivoMaterialConfiguration : IEntityTypeConfiguration<PlanMantenimientoPreventivoMaterial>
    {
        public void Configure(EntityTypeBuilder<PlanMantenimientoPreventivoMaterial> builder)
        {
            builder.ToTable("PlanesMantenimientoPreventivoMateriales", schema: "mantenimiento");

            builder.HasKey(x => new { x.PlantaCode, x.PlanCode, x.ArticuloCode });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.PlanCode).IsRequired().HasColumnType("char(6)");
            builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");

            builder.Property(x => x.Cantidad).HasColumnType("decimal(12,5)").HasDefaultValue(0m);
            builder.Property(x => x.CostoUnitario).HasColumnType("decimal(12,5)").HasDefaultValue(0m);
            builder.Property(x => x.CostoTotal).HasColumnType("decimal(12,5)").HasDefaultValue(0m);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Articulo).WithMany()
                .HasForeignKey(x => x.ArticuloCode).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
