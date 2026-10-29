using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class SolicitudNotaCreditoAprobacionConfiguration : IEntityTypeConfiguration<SolicitudNotaCreditoAprobacion>
    {
        public void Configure(EntityTypeBuilder<SolicitudNotaCreditoAprobacion> builder)
        {
            builder.ToTable("SolicitudesNotaCreditoAprobaciones", schema: "facturacion");

            builder.HasKey(x => new { x.Year, x.Month, x.Code, x.Stage });

            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Month).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.Stage).IsRequired().HasConversion<int>();

            builder.Property(x => x.Status).IsRequired().HasConversion<int>()
                .HasDefaultValue(EstadoAprobacionSolicitud.Pendiente)
                .HasSentinel((EstadoAprobacionSolicitud)(-1));

            builder.Property(x => x.ActedBy).HasMaxLength(250);
            builder.Property(x => x.Observation).HasMaxLength(200);
        }
    }
}
