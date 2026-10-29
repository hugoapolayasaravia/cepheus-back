// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/ImportacionDetalleConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class ImportacionDetalleConfiguration : IEntityTypeConfiguration<ImportacionDetalle>
    {
        public void Configure(EntityTypeBuilder<ImportacionDetalle> builder)
        {
            builder.ToTable("ImportacionDetalles", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.ImportacionCode, x.ProveedorCode, x.ArticuloCode });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ImportacionCode).IsRequired().HasColumnType("char(6)");

            builder.HasOne(x => x.Importacion)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => new { x.PlantaCode, x.ImportacionCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");
            builder.HasOne(x => x.Articulo).WithMany().HasForeignKey(x => x.ArticuloCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ComprobantePagoCode).IsRequired().HasColumnType("char(2)").IsFixedLength();
            builder.HasOne(x => x.ComprobantePago).WithMany().HasForeignKey(x => x.ComprobantePagoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.NumeroDocumento).IsRequired().HasMaxLength(15);
            builder.Property(x => x.FechaEmision).IsRequired();
            builder.Property(x => x.TipoCambio).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);

            builder.Property(x => x.Cantidad).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.ValorFob).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.Flete).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.Seguro).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.ValorAduana).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);

            builder.Property(x => x.PorcentajeDet).IsRequired().HasColumnType("decimal(18,10)").HasDefaultValue(0);
            builder.Property(x => x.ValorDet).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.AdvaloremDet).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.SobretasaDet).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.IgvDet).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.OtrosGastosDet).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);

            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => new { x.PlantaCode, x.ImportacionCode, x.Estado });
        }
    }
}
