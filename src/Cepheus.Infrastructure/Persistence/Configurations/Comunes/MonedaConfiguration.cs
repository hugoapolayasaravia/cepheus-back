using Cepheus.Domain.Comunes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Comunes
{
    public class MonedaConfiguration : IEntityTypeConfiguration<Moneda>
    {
        public void Configure(EntityTypeBuilder<Moneda> builder)
        {
            builder.ToTable("Monedas", schema: "comun");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(3)");

            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Symbol)
                .HasMaxLength(5);


            builder.Property(x => x.NumericCode)
                .HasMaxLength(3);


            builder.HasIndex(x => x.NumericCode).IsUnique();

            builder.Property(x => x.DecimalPlaces)
                .IsRequired()
                .HasDefaultValue(2);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}