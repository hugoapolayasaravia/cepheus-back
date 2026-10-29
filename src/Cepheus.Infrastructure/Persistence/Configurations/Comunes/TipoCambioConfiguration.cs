using Cepheus.Domain.Comunes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Comunes
{
    public class TipoCambioConfiguration : IEntityTypeConfiguration<TipoCambio>
    {
        public void Configure(EntityTypeBuilder<TipoCambio> builder)
        {
            builder.ToTable("TiposCambio", schema: "comun");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Date)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.MonedaCode)
                .IsRequired()
                .HasColumnType("char(3)");

            // Un tipo de cambio por moneda y fecha.
            builder.HasIndex(x => new { x.MonedaCode, x.Date }).IsUnique();

            builder.HasOne(x => x.Moneda)
                .WithMany()
                .HasForeignKey(x => x.MonedaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.SellRate)
                .IsRequired()
                .HasColumnType("numeric(9,4)");

            builder.Property(x => x.BuyRate)
                .IsRequired()
                .HasColumnType("numeric(9,4)");

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
