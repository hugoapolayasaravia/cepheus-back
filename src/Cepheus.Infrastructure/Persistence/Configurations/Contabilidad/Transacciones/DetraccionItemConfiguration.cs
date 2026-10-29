using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class DetraccionItemConfiguration : IEntityTypeConfiguration<DetraccionItem>
    {
        public void Configure(EntityTypeBuilder<DetraccionItem> builder)
        {
            builder.ToTable("DetraccionItems", schema: "contabilidad");

            builder.HasKey(x => new { x.Year, x.Period, x.LotNumber, x.ProveedorCode, x.ItemNumber });

            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Period).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.LotNumber).IsRequired().HasColumnType("char(4)");

            builder.Property(x => x.ItemNumber).IsRequired();

            builder.Property(x => x.ProveedorRuc).IsRequired().HasColumnType("char(11)");
            builder.Property(x => x.AccountNumber).IsRequired().HasMaxLength(30);

            builder.Property(x => x.DepositSol).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorCode)
                .HasPrincipalKey(p => p.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TipoOperacion).WithMany().HasForeignKey(x => x.TipoOperacionCode)
                .HasPrincipalKey(t => t.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TipoBien).WithMany().HasForeignKey(x => x.TipoBienCode)
                .HasPrincipalKey(t => t.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ProveedorCuenta).WithMany().HasForeignKey(x => x.ProveedorCuentaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Documents).WithOne(d => d.Item)
                .HasForeignKey(d => new { d.Year, d.Period, d.LotNumber, d.ProveedorCode, d.ItemNumber })
                .HasPrincipalKey(i => new { i.Year, i.Period, i.LotNumber, i.ProveedorCode, i.ItemNumber })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ProveedorCode);
            builder.HasIndex(x => x.TipoOperacionCode);
            builder.HasIndex(x => x.TipoBienCode);
        }
    }
}
