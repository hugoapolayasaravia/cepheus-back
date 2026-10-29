using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
    {
        public void Configure(EntityTypeBuilder<Vehiculo> builder)
        {
            builder.ToTable("Vehiculos", schema: "logistica");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(5)");

            builder.Property(x => x.TransportistaCode)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(x => x.LicensePlate)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(x => x.LicensePlate).IsUnique();

            builder.Property(x => x.VehicleCategory).HasMaxLength(10);
            builder.Property(x => x.VehicleType).HasMaxLength(30);
            builder.Property(x => x.Brand).HasMaxLength(50);
            builder.Property(x => x.Model).HasMaxLength(50);
            builder.Property(x => x.EngineNumber).HasMaxLength(50);
            builder.Property(x => x.ChassisNumber).HasMaxLength(50);
            builder.Property(x => x.Color).HasMaxLength(30);

            builder.Property(x => x.CargoCapacityKg).HasColumnType("decimal(12,2)");
            builder.Property(x => x.LengthM).HasColumnType("decimal(10,2)");
            builder.Property(x => x.WidthM).HasColumnType("decimal(10,2)");
            builder.Property(x => x.HeightM).HasColumnType("decimal(10,2)");

            builder.Property(x => x.VehicularCertificateNumber).HasMaxLength(50);
            builder.Property(x => x.CirculationCardNumber).HasMaxLength(50);
            builder.Property(x => x.VehicularConfiguration).HasMaxLength(30);

            builder.Property(x => x.Observations).HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Transportista)
                .WithMany()
                .HasForeignKey(x => x.TransportistaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TransportistaCode);
        }
    }
}
