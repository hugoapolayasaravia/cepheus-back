using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class CotizacionMetradoDetalleConfiguration : IEntityTypeConfiguration<CotizacionMetradoDetalle>
    {
        public void Configure(EntityTypeBuilder<CotizacionMetradoDetalle> builder)
        {
            builder.ToTable("CotizacionesMetradoDetalle", schema: "facturacion");

            builder.HasKey(x => new
            {
                x.NegocioCode, x.Year, x.Month, x.Code, x.LevelNumber, x.Order,
                x.ProductoTipoCode, x.ProductoCode
            });

            builder.Property(x => x.NegocioCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Month).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.LevelNumber).IsRequired();

            builder.Property(x => x.Order).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.ProductoTipoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ProductoCode).IsRequired().HasColumnType("char(4)");

            builder.Property(x => x.PanelCode).IsRequired().HasMaxLength(5).HasDefaultValue("");
            builder.Property(x => x.SortOrder).IsRequired().HasColumnType("char(3)").HasDefaultValue("00");

            // decimal(18,6): magnitudes de metrado (longitudes, cantidades, áreas)
            foreach (var prop in new[]
            {
                nameof(CotizacionMetradoDetalle.InnerLength),
                nameof(CotizacionMetradoDetalle.OuterLength),
                nameof(CotizacionMetradoDetalle.Support),
                nameof(CotizacionMetradoDetalle.Quantity),
                nameof(CotizacionMetradoDetalle.TotalMaterial),
                nameof(CotizacionMetradoDetalle.VaultCount),
                nameof(CotizacionMetradoDetalle.WastePercentage),
                nameof(CotizacionMetradoDetalle.Row),
                nameof(CotizacionMetradoDetalle.QuantityB),
                nameof(CotizacionMetradoDetalle.Support2),
                nameof(CotizacionMetradoDetalle.Width),
                nameof(CotizacionMetradoDetalle.Area),
                nameof(CotizacionMetradoDetalle.DeliveredTotalMaterial),
                nameof(CotizacionMetradoDetalle.DeliveredQuantityB),
                nameof(CotizacionMetradoDetalle.Widening),
                nameof(CotizacionMetradoDetalle.SupportP),
                nameof(CotizacionMetradoDetalle.WastePercentageP),
                nameof(CotizacionMetradoDetalle.QuantityP),
            })
            {
                builder.Property(typeof(decimal), prop).IsRequired().HasColumnType("decimal(18,6)").HasDefaultValue(0m);
            }

            // decimal(18,4): precios unitarios
            foreach (var prop in new[]
            {
                nameof(CotizacionMetradoDetalle.MaterialPrice),
                nameof(CotizacionMetradoDetalle.TransportPrice),
                nameof(CotizacionMetradoDetalle.VaultPrice),
                nameof(CotizacionMetradoDetalle.VaultTotalPrice),
                nameof(CotizacionMetradoDetalle.MaterialPriceAlt),
                nameof(CotizacionMetradoDetalle.TransportPriceAlt),
                nameof(CotizacionMetradoDetalle.VaultPriceAlt),
                nameof(CotizacionMetradoDetalle.VaultTotalPriceAlt),
                nameof(CotizacionMetradoDetalle.PolystyrenePrice),
                nameof(CotizacionMetradoDetalle.PolystyreneTotalPrice),
                nameof(CotizacionMetradoDetalle.PolystyrenePriceAlt),
                nameof(CotizacionMetradoDetalle.PolystyreneTotalPriceAlt),
            })
            {
                builder.Property(typeof(decimal), prop).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0m);
            }

            // decimal(18,2): porcentajes/importes de IGV y espaciamiento
            builder.Property(x => x.MaterialIgv).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.Spacing).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.Times).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.Anchorage).IsRequired().HasConversion<int>().HasDefaultValue(Cepheus.Domain.Facturacion.Enum.TipoAnclaje.Si).HasSentinel(default(Cepheus.Domain.Facturacion.Enum.TipoAnclaje));

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.MetradoResumen).WithMany(r => r.Detalles)
                .HasForeignKey(x => new { x.NegocioCode, x.Year, x.Month, x.Code, x.LevelNumber })
                .HasPrincipalKey(r => new { r.NegocioCode, r.Year, r.Month, r.Code, r.LevelNumber })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Producto).WithMany()
                .HasForeignKey(x => new { x.ProductoTipoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
