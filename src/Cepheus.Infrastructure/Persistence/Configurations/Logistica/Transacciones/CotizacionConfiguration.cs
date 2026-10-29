// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/CotizacionConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class CotizacionConfiguration : IEntityTypeConfiguration<Cotizacion>
    {
        public void Configure(EntityTypeBuilder<Cotizacion> builder)
        {
            builder.ToTable("Cotizaciones", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.Code });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.Planta).WithMany().HasForeignKey(x => x.PlantaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Code).IsRequired().HasColumnType("char(6)");

            builder.Property(x => x.FechaLimite).IsRequired();

            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();

            builder.Property(x => x.Observaciones).IsRequired().HasMaxLength(200).HasDefaultValue(string.Empty);

            builder.Property(x => x.OriginalCode).HasColumnType("char(6)");
            builder.HasOne(x => x.Original)
                .WithMany()
                .HasForeignKey(x => new { x.PlantaCode, x.OriginalCode })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasMany(x => x.Detalles)
                .WithOne(x => x.Cotizacion)
                .HasForeignKey(x => new { x.PlantaCode, x.CotizacionCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Proveedores)
                .WithOne(x => x.Cotizacion)
                .HasForeignKey(x => new { x.PlantaCode, x.CotizacionCode })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}