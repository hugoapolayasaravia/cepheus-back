// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/GuiaDetalleConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class GuiaDetalleConfiguration : IEntityTypeConfiguration<GuiaDetalle>
    {
        public void Configure(EntityTypeBuilder<GuiaDetalle> builder)
        {
            builder.ToTable("GuiaDetalles", schema: "logistica");

            // PK legacy (Codigo_pla, Numero_gve, Codigo_art): un artículo no se repite en la guía.
            builder.HasKey(x => new { x.PlantaCode, x.GuiaCode, x.ArticuloCode });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.GuiaCode).IsRequired().HasColumnType("char(10)");

            builder.HasOne(x => x.Guia)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => new { x.PlantaCode, x.GuiaCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");
            builder.HasOne(x => x.Articulo)
                .WithMany()
                .HasForeignKey(x => x.ArticuloCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ItemNumber).IsRequired();

            builder.Property(x => x.Cantidad).IsRequired().HasColumnType("decimal(12,2)");

            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();

            builder.Property(x => x.IsVerified).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
