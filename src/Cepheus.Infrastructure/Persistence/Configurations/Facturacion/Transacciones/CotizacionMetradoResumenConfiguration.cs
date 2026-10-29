using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class CotizacionMetradoResumenConfiguration : IEntityTypeConfiguration<CotizacionMetradoResumen>
    {
        public void Configure(EntityTypeBuilder<CotizacionMetradoResumen> builder)
        {
            builder.ToTable("CotizacionesMetradoResumen", schema: "facturacion");

            builder.HasKey(x => new { x.NegocioCode, x.Year, x.Month, x.Code, x.LevelNumber });

            builder.Property(x => x.NegocioCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Month).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");

            builder.Property(x => x.LevelNumber).IsRequired();

            builder.Property(x => x.LevelName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.AlturaLosaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.OverloadOrShortage).IsRequired().HasMaxLength(50);

            foreach (var prop in new[]
            {
                nameof(CotizacionMetradoResumen.LinealMeters),
                nameof(CotizacionMetradoResumen.TotalVaults),
                nameof(CotizacionMetradoResumen.TotalMeters),
                nameof(CotizacionMetradoResumen.TotalPrice),
                nameof(CotizacionMetradoResumen.PricePerM2),
                nameof(CotizacionMetradoResumen.TotalVaultsAlt),
                nameof(CotizacionMetradoResumen.TotalMetersAlt),
                nameof(CotizacionMetradoResumen.TotalPriceAlt),
                nameof(CotizacionMetradoResumen.PricePerM2Alt),
                nameof(CotizacionMetradoResumen.MinTotal),
                nameof(CotizacionMetradoResumen.MinTotalAlt),
                nameof(CotizacionMetradoResumen.MinTotalB),
                nameof(CotizacionMetradoResumen.MinTotalAltB),
            })
            {
                builder.Property(typeof(decimal), prop).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            }

            builder.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(18,0)").HasDefaultValue(0m);
            builder.Property(x => x.BuildingLevel).IsRequired().HasMaxLength(50);

            builder.Property(x => x.HasTransport).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.HasMinPrice).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.HasTransportB).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.HasMinPriceB).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Cotizacion).WithMany(c => c.MetradoResumenes)
                .HasForeignKey(x => new { x.NegocioCode, x.Year, x.Month, x.Code })
                .HasPrincipalKey(c => new { c.NegocioCode, c.Year, c.Month, c.Code })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AlturaLosa).WithMany()
                .HasForeignKey(x => x.AlturaLosaCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
