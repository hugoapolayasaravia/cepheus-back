// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/ImportacionGastoConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class ImportacionGastoConfiguration : IEntityTypeConfiguration<ImportacionGasto>
    {
        public void Configure(EntityTypeBuilder<ImportacionGasto> builder)
        {
            builder.ToTable("ImportacionGastos", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.ImportacionCode, x.ProveedorCode, x.NumeroDocumento });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ImportacionCode).IsRequired().HasColumnType("char(6)");

            builder.HasOne(x => x.Importacion)
                .WithMany(x => x.Gastos)
                .HasForeignKey(x => new { x.PlantaCode, x.ImportacionCode })
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.NumeroDocumento).IsRequired().HasMaxLength(15);

            builder.Property(x => x.ComprobantePagoCode).IsRequired().HasColumnType("char(2)").IsFixedLength();
            builder.HasOne(x => x.ComprobantePago).WithMany().HasForeignKey(x => x.ComprobantePagoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.MonedaCode).IsRequired().HasMaxLength(3);
            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Afecto).IsRequired();
            builder.Property(x => x.FechaEmision).IsRequired();
            builder.Property(x => x.TipoCambio).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);

            builder.Property(x => x.NetoGasto).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.NetoGastoInafecto).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.Igv).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.IgvExterior).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.Total).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }

    public class ImportacionGastoArticuloConfiguration : IEntityTypeConfiguration<ImportacionGastoArticulo>
    {
        public void Configure(EntityTypeBuilder<ImportacionGastoArticulo> builder)
        {
            builder.ToTable("ImportacionGastoArticulos", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.ImportacionCode, x.ProveedorCode, x.NumeroDocumento, x.ArticuloCode });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ImportacionCode).IsRequired().HasColumnType("char(6)");
            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.NumeroDocumento).IsRequired().HasMaxLength(15);
            builder.Property(x => x.ArticuloCode).IsRequired().HasColumnType("char(7)");

            builder.HasOne(x => x.Gasto)
                .WithMany(x => x.Articulos)
                .HasForeignKey(x => new { x.PlantaCode, x.ImportacionCode, x.ProveedorCode, x.NumeroDocumento })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Articulo).WithMany().HasForeignKey(x => x.ArticuloCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ValorGasto).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.IgvGasto).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
            builder.Property(x => x.IgvExtGasto).IsRequired().HasColumnType("decimal(12,4)").HasDefaultValue(0);
        }
    }
}
