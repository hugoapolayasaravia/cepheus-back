using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class AsientoContableDetalleConfiguration : IEntityTypeConfiguration<AsientoContableDetalle>
    {
        public void Configure(EntityTypeBuilder<AsientoContableDetalle> builder)
        {
            builder.ToTable("AsientoContableDetalles", schema: "contabilidad");

            builder.HasKey(x => new { x.Anio, x.Mes, x.TipoAsientoCode, x.Numero, x.ItemNumber });

            builder.Property(x => x.Anio).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Mes).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.TipoAsientoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Numero).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.ItemNumber).IsRequired();

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.Planta).WithMany().HasForeignKey(x => x.PlantaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.EsDestino).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(50);

            builder.Property(x => x.CuentaContableCode).IsRequired().HasColumnType("char(8)");
            builder.HasOne(x => x.CuentaContable).WithMany().HasForeignKey(x => x.CuentaContableCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AnexoTipoCode).HasColumnType("char(3)");
            builder.Property(x => x.AnexoCode).HasColumnType("char(5)");
            builder.HasOne(x => x.Anexo).WithMany()
                .HasForeignKey(x => new { x.AnexoTipoCode, x.AnexoCode })
                .HasPrincipalKey(a => new { a.TipoAnexoCode, a.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.NumeroDocumento).HasMaxLength(15);

            builder.Property(x => x.CargoSol).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.AbonoSol).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);

            builder.Property(x => x.TipoContabilidad).IsRequired().HasConversion<int>();

            builder.Property(x => x.PlantaEjecutoraCode).HasColumnType("char(2)");
            builder.HasOne(x => x.PlantaEjecutora).WithMany().HasForeignKey(x => x.PlantaEjecutoraCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ComprobantePagoCode).HasMaxLength(2);
            builder.HasOne(x => x.ComprobantePago).WithMany().HasForeignKey(x => x.ComprobantePagoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => x.CuentaContableCode);
            builder.HasIndex(x => x.NumeroDocumento);
        }
    }
}
