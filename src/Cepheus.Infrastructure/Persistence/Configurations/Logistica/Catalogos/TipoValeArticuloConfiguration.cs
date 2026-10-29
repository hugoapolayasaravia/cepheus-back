using Cepheus.Domain.Logistica.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Catalogos;

public sealed class TipoValeArticuloConfiguration : IEntityTypeConfiguration<TipoValeArticulo>
{
    public void Configure(EntityTypeBuilder<TipoValeArticulo> builder)
    {
        builder.ToTable("TiposValeArticulo", "logistica");

        builder.HasKey(x => new { x.TipoValeCode, x.ArticuloCode });

        builder.Property(x => x.TipoValeCode).HasColumnType("char(3)").IsRequired();
        builder.Property(x => x.ArticuloCode).HasColumnType("char(7)").IsRequired();

        builder.HasOne(x => x.TipoVale)
            .WithMany()
            .HasForeignKey(x => x.TipoValeCode)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Articulo)
            .WithMany()
            .HasForeignKey(x => x.ArticuloCode)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);

        builder.HasIndex(x => x.ArticuloCode);
    }
}
