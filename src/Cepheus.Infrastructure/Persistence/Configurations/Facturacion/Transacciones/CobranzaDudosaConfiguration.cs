using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class CobranzaDudosaConfiguration : IEntityTypeConfiguration<CobranzaDudosa>
    {
        public void Configure(EntityTypeBuilder<CobranzaDudosa> builder)
        {
            builder.ToTable("CobranzasDudosas", schema: "facturacion");

            builder.HasKey(x => new { x.TypeCode, x.PlantaCode, x.Number, x.ClienteCode, x.BancoCode });

            builder.Property(x => x.TypeCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("varchar(2)");
            builder.Property(x => x.Number).IsRequired().HasMaxLength(15);
            builder.Property(x => x.ClienteCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.BancoCode).IsRequired().HasColumnType("varchar(3)");
            builder.Property(x => x.ObraCode).HasColumnType("char(3)");

            builder.Property(x => x.ProcessDate).IsRequired().HasColumnType("date");
            builder.Property(x => x.SystemDate).IsRequired().HasColumnType("date");

            builder.Property(x => x.TipoContabilidad).IsRequired();
            builder.Property(x => x.CurrencyCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.ExchangeRate).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(x => x.TotalSoles).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.TotalDollars).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.ProvisionEntry).HasMaxLength(13);
            builder.Property(x => x.CanjeEntry).HasMaxLength(13);

            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.WrittenOffAt).HasColumnType("date");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => x.CanjeEntry);
            builder.HasIndex(x => x.ClienteCode);
        }
    }
}
