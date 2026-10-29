using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public sealed class OrdenServicioSalidaConfiguration : IEntityTypeConfiguration<OrdenServicioSalida>
{
    public void Configure(EntityTypeBuilder<OrdenServicioSalida> builder)
    {
        builder.ToTable("OrdenesServicioSalida", "logistica");

        builder.HasKey(x => new { x.PlantaCode, x.Code });

        builder.Property(x => x.PlantaCode).HasColumnType("char(2)").IsRequired();
        builder.Property(x => x.Code).HasColumnType("char(6)").IsRequired();

        builder.HasOne(x => x.Planta)
            .WithMany()
            .HasForeignKey(x => x.PlantaCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.FechaEntrega).IsRequired();

        builder.Property(x => x.TrabajadorCode).HasColumnType("char(5)").IsRequired();
        builder.HasOne(x => x.Trabajador)
            .WithMany()
            .HasForeignKey(x => x.TrabajadorCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Neto).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Igv).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Total).HasColumnType("decimal(12,2)").IsRequired();

        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();

        builder.Property(x => x.AsientoContable).HasMaxLength(13);
        builder.Property(x => x.AprobadoPor).HasMaxLength(250);
        builder.Property(x => x.ProcesadoPor).HasMaxLength(250);

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasIndex(x => new { x.PlantaCode, x.CreatedAt });

        builder.HasMany(x => x.Detalles)
            .WithOne(x => x.Salida)
            .HasForeignKey(x => new { x.PlantaCode, x.SalidaCode })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
