using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class AsientoContableConfiguration : IEntityTypeConfiguration<AsientoContable>
    {
        public void Configure(EntityTypeBuilder<AsientoContable> builder)
        {
            builder.ToTable("AsientosContables", schema: "contabilidad");

            builder.HasKey(x => new { x.Anio, x.Mes, x.TipoAsientoCode, x.Numero });

            builder.Ignore(x => x.Codigo);

            builder.Property(x => x.Anio).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Mes).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Numero).IsRequired().HasColumnType("char(5)");

            builder.Property(x => x.TipoAsientoCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.TipoAsiento).WithMany().HasForeignKey(x => x.TipoAsientoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(500);

            builder.Property(x => x.FechaProceso).IsRequired().HasColumnType("date");

            builder.Property(x => x.TipoCambio).IsRequired().HasColumnType("decimal(10,4)").HasDefaultValue(0);
            builder.Property(x => x.TotalCargosSol).IsRequired().HasColumnType("decimal(15,2)").HasDefaultValue(0);
            builder.Property(x => x.TotalAbonosSol).IsRequired().HasColumnType("decimal(15,2)").HasDefaultValue(0);

            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();
            builder.Property(x => x.Origen).IsRequired().HasConversion<int>();

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => new { x.Anio, x.Mes });
            builder.HasIndex(x => x.Estado);
            builder.HasIndex(x => x.Origen);
            builder.HasIndex(x => x.FechaProceso);

            builder.HasMany(x => x.Detalles)
                .WithOne(x => x.Asiento)
                .HasForeignKey(x => new { x.Anio, x.Mes, x.TipoAsientoCode, x.Numero })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
