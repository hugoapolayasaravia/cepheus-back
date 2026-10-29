using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ArticuloStockConfiguration : IEntityTypeConfiguration<ArticuloStock>
    {
        public void Configure(EntityTypeBuilder<ArticuloStock> builder)
        {
            builder.ToTable("StockArticulos", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.ArticuloCode });

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                 .HasColumnType("char(2)");

            builder.Property(x => x.ArticuloCode)
                .IsRequired()
                 .HasColumnType("char(7)");

            builder.Property(x => x.Quantity)
                .IsRequired()
                .HasColumnType("decimal(12,2)")
                .HasDefaultValue(0);

            builder.Property(x => x.UnitCost)
                .IsRequired()
                .HasColumnType("decimal(18,6)")
                .HasDefaultValue(0);

            builder.Property(x => x.UnitCostUsd)
                .IsRequired()
                .HasColumnType("decimal(18,6)")
                .HasDefaultValue(0);

            builder.Property(x => x.AverageCost)
                .HasColumnType("decimal(18,6)");

            builder.Property(x => x.MinStock)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(x => x.MaxStock)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Articulo)
                .WithMany()
                .HasForeignKey(x => x.ArticuloCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // CHECK a nivel de base de datos: última línea de defensa contra
            // stock negativo, incluso si algún proceso escribe fuera de la API.
            builder.ToTable(t => t.HasCheckConstraint("CK_StockArticulos_Quantity_NonNegative", "[Quantity] >= 0"));
        }
    }
}
