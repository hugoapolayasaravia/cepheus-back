using Cepheus.Domain.Facturacion.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Maestros
{
    public class VehiculoVentaConfiguration : IEntityTypeConfiguration<VehiculoVenta>
    {
        public void Configure(EntityTypeBuilder<VehiculoVenta> builder)
        {
            builder.ToTable("Vehiculos", schema: "facturacion");

            builder.HasKey(x => new { x.TransportistaCode, x.VehicleType, x.Code });

            builder.Property(x => x.TransportistaCode)
                .IsRequired()
                 .HasColumnType("char(4)");

            builder.Property(x => x.VehicleType).IsRequired().HasConversion<int>();
            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(4)");

            builder.Property(x => x.LicensePlate).IsRequired().HasMaxLength(10);
            builder.HasIndex(x => new { x.TransportistaCode, x.LicensePlate }).IsUnique();

            builder.Property(x => x.Brand).HasMaxLength(50);
            builder.Property(x => x.Model).HasMaxLength(50);

            builder.Property(x => x.ChoferCode).HasMaxLength(4);

            builder.Property(x => x.Capacity).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);
            builder.Property(x => x.Suple).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);
            builder.Property(x => x.LengthM).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);
            builder.Property(x => x.WidthM).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);
            builder.Property(x => x.HeightM).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);
            builder.Property(x => x.Telescopic).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);
            builder.Property(x => x.WithoutSuple).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);
            builder.Property(x => x.WithSuple).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);
            builder.Property(x => x.CubicWithoutSuple).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);
            builder.Property(x => x.CubicWithSuple).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0m);

            builder.Property(x => x.MtcInternalCode).HasMaxLength(20);
            builder.Property(x => x.VehicularConfiguration).HasMaxLength(30);
            builder.Property(x => x.PlanillaCode).HasMaxLength(10);
            builder.Property(x => x.Observations).HasMaxLength(500);

            builder.Property(x => x.ApprovedBy).HasMaxLength(50);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.TransportistaVenta)
                .WithMany()
                .HasForeignKey(x => x.TransportistaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // FK compuesta opcional al chofer habitual: se valida solo cuando
            // ChoferCode tiene valor (SQL Server no evalúa la FK si alguna columna es NULL).
            builder.HasOne(x => x.ChoferVenta)
                .WithMany()
                .HasForeignKey(x => new { x.TransportistaCode, x.ChoferCode })
                .HasPrincipalKey(c => new { c.TransportistaCode, c.Code })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
