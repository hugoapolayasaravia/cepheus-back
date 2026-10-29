// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/OrdenCompraAdministracionDetalleConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class OrdenCompraAdministracionDetalleConfiguration : IEntityTypeConfiguration<OrdenCompraAdministracionDetalle>
    {
        public void Configure(EntityTypeBuilder<OrdenCompraAdministracionDetalle> builder)
        {
            builder.ToTable("OrdenCompraAdministracionDetalles", schema: "logistica");

            builder.HasKey(x => new { x.OrdenCompraAdministracionCode, x.ItemNumber });

            builder.Property(x => x.OrdenCompraAdministracionCode).IsRequired().HasColumnType("char(6)");

            builder.HasOne(x => x.OrdenCompraAdministracion)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.OrdenCompraAdministracionCode)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ItemNumber).IsRequired();

            builder.Property(x => x.Glosa).IsRequired().HasMaxLength(500);

            builder.Property(x => x.UnidadMedidaCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.UnidadMedida).WithMany().HasForeignKey(x => x.UnidadMedidaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Cantidad).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.Precio).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0);
            builder.Property(x => x.Descuento).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.Total).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.CantidadEntregada).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);

            builder.Property(x => x.SubCentroCostoCode).IsRequired().HasMaxLength(6);
            builder.HasOne(x => x.SubCentroCosto).WithMany().HasForeignKey(x => x.SubCentroCostoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
