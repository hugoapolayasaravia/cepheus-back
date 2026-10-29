// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/ImportacionConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class ImportacionConfiguration : IEntityTypeConfiguration<Importacion>
    {
        public void Configure(EntityTypeBuilder<Importacion> builder)
        {
            builder.ToTable("Importaciones", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.Code });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.Planta).WithMany().HasForeignKey(x => x.PlantaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Code).IsRequired().HasColumnType("char(6)");

            builder.Property(x => x.PesoNeto).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.PesoBruto).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);

            builder.Property(x => x.FechaPoliza).IsRequired();
            builder.Property(x => x.FechaEntrega);

            builder.Property(x => x.TipoCambio).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);

            builder.Property(x => x.TotalFob).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.TotalFlete).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.TotalSeguro).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.TotalAduana).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.Advalorem).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.Sobretasa).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.Igv).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.OtrosGastos).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);

            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => new { x.PlantaCode, x.CreatedAt });
            builder.HasIndex(x => new { x.PlantaCode, x.Estado });
        }
    }
}
