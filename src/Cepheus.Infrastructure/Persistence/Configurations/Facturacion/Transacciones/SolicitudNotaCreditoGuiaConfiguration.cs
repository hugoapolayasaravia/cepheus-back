using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class SolicitudNotaCreditoGuiaConfiguration : IEntityTypeConfiguration<SolicitudNotaCreditoGuia>
    {
        public void Configure(EntityTypeBuilder<SolicitudNotaCreditoGuia> builder)
        {
            builder.ToTable("SolicitudesNotaCreditoGuias", schema: "facturacion");

            builder.HasKey(x => new { x.Year, x.Month, x.Code, x.GuideNumber });

            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Month).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.GuideNumber).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        }
    }
}
