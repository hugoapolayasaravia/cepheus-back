using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class SolicitudNotaCreditoArchivoConfiguration : IEntityTypeConfiguration<SolicitudNotaCreditoArchivo>
    {
        public void Configure(EntityTypeBuilder<SolicitudNotaCreditoArchivo> builder)
        {
            builder.ToTable("SolicitudesNotaCreditoArchivos", schema: "facturacion");

            builder.HasKey(x => new { x.Year, x.Month, x.Code, x.FileName });
            
            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Month).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.FileName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.FilePath).IsRequired().HasMaxLength(500);
        }
    }
}
