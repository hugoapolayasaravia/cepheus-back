using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public sealed class OrdenServicioDetalleConfiguration : IEntityTypeConfiguration<OrdenServicioDetalle>
{
    public void Configure(EntityTypeBuilder<OrdenServicioDetalle> builder)
    {
        builder.ToTable("OrdenServicioDetalles", "logistica");

        // Mismo criterio que NotaIngresoDetalle / PedidoDetalle: correlativo de línea asignado por el servidor.
        builder.HasKey(x => new { x.PlantaCode, x.OrdenServicioCode, x.ItemNumber });

        builder.Property(x => x.PlantaCode).HasColumnType("char(2)").IsRequired();
        builder.Property(x => x.OrdenServicioCode).HasColumnType("char(6)").IsRequired();
        builder.Property(x => x.ItemNumber).IsRequired();

        builder.Property(x => x.ArticuloCode).HasColumnType("char(7)").IsRequired();
        builder.HasOne(x => x.Articulo)
            .WithMany()
            .HasForeignKey(x => x.ArticuloCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Glosa); // nvarchar(max), como Glosa_Art legacy (varchar(max))

        builder.Property(x => x.Cantidad).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Precio).HasColumnType("decimal(18,6)").IsRequired();
        builder.Property(x => x.Descuento).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Total).HasColumnType("decimal(12,2)").IsRequired();

        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();

        builder.Property(x => x.TipoValeCode).HasColumnType("char(3)").IsRequired();
        builder.HasOne(x => x.TipoVale)
            .WithMany()
            .HasForeignKey(x => x.TipoValeCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.SubCentroCostoCode).HasColumnType("char(6)").IsRequired();
        builder.HasOne(x => x.SubCentroCosto)
            .WithMany()
            .HasForeignKey(x => x.SubCentroCostoCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.SubCentroEjecutorCode).HasColumnType("char(4)");
        builder.HasOne(x => x.SubCentroEjecutor)
            .WithMany()
            .HasForeignKey(x => x.SubCentroEjecutorCode)
            .OnDelete(DeleteBehavior.Restrict);

        // OT de la misma planta de la orden (FK compuesta nulable, mismo criterio que NotaIngresoDetalle -> Pedido).
        builder.Property(x => x.OrdenTrabajoCode).HasColumnType("char(6)");
        builder.HasOne(x => x.OrdenTrabajo)
            .WithMany()
            .HasForeignKey(x => new { x.PlantaCode, x.OrdenTrabajoCode })
            .HasPrincipalKey(x => new { x.PlantaCode, x.Code })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.PlantaAfectadaCode).HasColumnType("char(2)").IsRequired();
        builder.HasOne(x => x.PlantaAfectada)
            .WithMany()
            .HasForeignKey(x => x.PlantaAfectadaCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
