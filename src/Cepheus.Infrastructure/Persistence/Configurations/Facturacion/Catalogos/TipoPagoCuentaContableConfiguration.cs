using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class TipoPagoCuentaContableConfiguration : IEntityTypeConfiguration<TipoPagoCuentaContable>
    {
        public void Configure(EntityTypeBuilder<TipoPagoCuentaContable> builder)
        {
            builder.ToTable("TiposPagoCuentaContable", schema: "facturacion");

            builder.HasKey(x => new { x.TipoPagoCode, x.MonedaCode });

            builder.Property(x => x.TipoPagoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.MonedaCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.AccountCode).IsRequired().HasColumnType("char(8)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.HasOne(x => x.TipoPago).WithMany(t => t.Accounts).HasForeignKey(x => x.TipoPagoCode)
                .HasPrincipalKey(t => t.Code).OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .HasPrincipalKey(m => m.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
