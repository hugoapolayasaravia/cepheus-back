using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public sealed class ValeConfiguration : IEntityTypeConfiguration<Vale>
{
    public void Configure(EntityTypeBuilder<Vale> builder)
    {
        builder.ToTable("Vales", "logistica");

        builder.HasKey(x => new { x.PlantaCode, x.Code });

        builder.Property(x => x.PlantaCode).HasColumnType("char(2)").IsRequired();
        builder.Property(x => x.Code).HasColumnType("char(7)").IsRequired();

        builder.HasOne(x => x.Planta)
            .WithMany()
            .HasForeignKey(x => x.PlantaCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TipoValeCode).HasColumnType("char(3)").IsRequired();
        builder.HasOne(x => x.TipoVale)
            .WithMany()
            .HasForeignKey(x => x.TipoValeCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.FechaEntrega).IsRequired();

        builder.Property(x => x.SubCentroCostoCode).HasColumnType("char(6)").IsRequired();
        builder.HasOne(x => x.SubCentroCosto)
            .WithMany()
            .HasForeignKey(x => x.SubCentroCostoCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.SubCentroEjecutorCode).HasColumnType("char(4)");
        builder.HasOne(x => x.SubCentroEjecutor)
            .WithMany()
            .HasForeignKey(x => x.SubCentroEjecutorCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TrabajadorCode).HasColumnType("char(5)").IsRequired();
        builder.HasOne(x => x.Trabajador)
            .WithMany()
            .HasForeignKey(x => x.TrabajadorCode)
            .OnDelete(DeleteBehavior.Restrict);

        // La OT del vale pertenece a la misma planta (almacén) que el vale (legacy: join por Codigo_Pla + Codigo_Otr).
        builder.Property(x => x.OrdenTrabajoCode).HasColumnType("char(6)");
        builder.HasOne(x => x.OrdenTrabajo)
            .WithMany()
            .HasForeignKey(x => new { x.PlantaCode, x.OrdenTrabajoCode })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UnidadNegocioCode).HasColumnType("char(6)").IsRequired();
        builder.HasOne(x => x.UnidadNegocio)
            .WithMany()
            .HasForeignKey(x => x.UnidadNegocioCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.PlantaAfectadaCode).HasColumnType("char(2)").IsRequired();
        builder.HasOne(x => x.PlantaAfectada)
            .WithMany()
            .HasForeignKey(x => x.PlantaAfectadaCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Neto).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Igv).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Total).HasColumnType("decimal(12,2)").IsRequired();

        // Enum persistido con el código legacy como valor (01, 09, 12, 13, 14, 30, 04).
        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();

        builder.Property(x => x.AsientoContable).HasMaxLength(13);
        builder.Property(x => x.AprobadoPor).HasMaxLength(250);
        builder.Property(x => x.ProcesadoPor).HasMaxLength(250);
        builder.Property(x => x.AnuladoPor).HasMaxLength(250);
        builder.Property(x => x.Preparado).IsRequired().HasDefaultValue(false);

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();

        // Búsquedas del listado legacy (Logi_sp_Listado_MVales).
        builder.HasIndex(x => new { x.PlantaCode, x.CreatedAt });
        builder.HasIndex(x => new { x.PlantaCode, x.Estado });
        builder.HasIndex(x => x.TrabajadorCode);

        builder.HasMany(x => x.Detalles)
            .WithOne(x => x.Vale)
            .HasForeignKey(x => new { x.PlantaCode, x.ValeCode })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
