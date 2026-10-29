using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public class SaldoMensualArticuloConfiguration : IEntityTypeConfiguration<SaldoMensualArticulo>
{
    public void Configure(EntityTypeBuilder<SaldoMensualArticulo> builder)
    {
        builder.ToTable("SaldosMensualesArticulo", schema: "logistica");

        builder.HasKey(x => new { x.PlantaCode, x.PeriodCode, x.ArticuloCode });

        builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
        builder.Property(x => x.PeriodCode).IsRequired().HasColumnType("char(6)");
        builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");

        builder.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);
        builder.Property(x => x.AverageCost).IsRequired().HasColumnType("decimal(18,6)").HasDefaultValue(0m);
        builder.Property(x => x.IncomingQuantity).IsRequired().HasColumnType("decimal(14,2)").HasDefaultValue(0m);
        builder.Property(x => x.OutgoingQuantity).IsRequired().HasColumnType("decimal(14,2)").HasDefaultValue(0m);


        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);

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

        // Saldo inicial de un artículo: último período cerrado anterior a una fecha.
        builder.HasIndex(x => new { x.PlantaCode, x.ArticuloCode, x.PeriodCode })
            .HasDatabaseName("IX_SaldosMensualesArticulo_Articulo");
    }
}
