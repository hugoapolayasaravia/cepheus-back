// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/PedidoConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.Code });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Code).IsRequired().HasColumnType("char(6)");

            builder.Property(x => x.CodPlanta).IsRequired().HasColumnType("char(2)");

            builder.Property(x => x.TipoPedidoCode).IsRequired().HasMaxLength(2);
            builder.HasOne(x => x.TipoPedido).WithMany().HasForeignKey(x => x.TipoPedidoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.TipoValeCode).HasMaxLength(3);
            builder.HasOne(x => x.TipoVale).WithMany().HasForeignKey(x => x.TipoValeCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.TramiteCode).IsRequired().HasMaxLength(1);
            builder.HasOne(x => x.Tramite).WithMany().HasForeignKey(x => x.TramiteCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.SubCentroCostoCode).IsRequired().HasMaxLength(6);
            builder.HasOne(x => x.SubCentroCosto).WithMany().HasForeignKey(x => x.SubCentroCostoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.TrabajadorCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.OrdenTrabajoCode).HasColumnType("char(6)");
            builder.HasOne(x => x.OrdenTrabajo)
                .WithMany()
                .HasForeignKey(x => new { x.PlantaCode, x.OrdenTrabajoCode })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.UnidadNegocioCode).IsRequired().HasMaxLength(6).HasDefaultValue("000000");
            builder.HasOne(x => x.UnidadNegocio).WithMany().HasForeignKey(x => x.UnidadNegocioCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.FechaEntrega).IsRequired();

            builder.Property(x => x.NetoPedido).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.IgvPedido).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.TotalPedido).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);

            builder.Property(x => x.EstadoPedido).IsRequired().HasConversion<int>();

            builder.Property(x => x.Observaciones).IsRequired().HasMaxLength(200).HasDefaultValue(string.Empty);

            builder.Property(x => x.AprobadoPor).HasMaxLength(20);
            builder.Property(x => x.CompradoPor).HasMaxLength(20);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasMany(x => x.Detalles)
                .WithOne(x => x.Pedido)
                .HasForeignKey(x => new { x.PlantaCode, x.PedidoCode })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}