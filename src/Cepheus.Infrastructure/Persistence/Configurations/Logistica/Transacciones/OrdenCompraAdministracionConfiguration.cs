// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/OrdenCompraAdministracionConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class OrdenCompraAdministracionConfiguration : IEntityTypeConfiguration<OrdenCompraAdministracion>
    {
        public void Configure(EntityTypeBuilder<OrdenCompraAdministracion> builder)
        {
            builder.ToTable("OrdenesCompraAdministracion", schema: "logistica");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code).IsRequired().HasColumnType("char(6)");

            builder.Property(x => x.ComprobantePagoCode).IsRequired().HasColumnType("char(2)").IsFixedLength();
            builder.HasOne(x => x.ComprobantePago).WithMany().HasForeignKey(x => x.ComprobantePagoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.EmitirDocumento).HasMaxLength(30);

            builder.Property(x => x.FechaEntrega).IsRequired();

            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CompradorCode).IsRequired().HasMaxLength(3);
            builder.HasOne(x => x.Comprador).WithMany().HasForeignKey(x => x.CompradorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.MonedaCode).IsRequired().HasMaxLength(3);
            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.LugarEnvioCode).IsRequired().HasMaxLength(3);
            builder.HasOne(x => x.LugarEnvio).WithMany().HasForeignKey(x => x.LugarEnvioCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.FormaPagoCode).IsRequired().HasMaxLength(2);
            builder.HasOne(x => x.FormaPago).WithMany().HasForeignKey(x => x.FormaPagoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.TramiteCode).IsRequired().HasMaxLength(1);
            builder.HasOne(x => x.Tramite).WithMany().HasForeignKey(x => x.TramiteCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Observaciones1).HasMaxLength(200);
            builder.Property(x => x.Observaciones2).HasMaxLength(200);

            builder.Property(x => x.NotaCompraCode).HasMaxLength(3);
            builder.HasOne(x => x.NotaCompra).WithMany().HasForeignKey(x => x.NotaCompraCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.UnidadNegocioCode).IsRequired().HasMaxLength(6).HasDefaultValue("000000");
            builder.HasOne(x => x.UnidadNegocio).WithMany().HasForeignKey(x => x.UnidadNegocioCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Motivo).IsRequired().HasConversion<int>();
            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();

            builder.Property(x => x.AprobadoPor).HasMaxLength(250);

            builder.Property(x => x.NetoCompra).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);
            builder.Property(x => x.IgvCompra).IsRequired().HasColumnType("decimal(12,5)").HasDefaultValue(0);
            builder.Property(x => x.TotalCompra).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.NoGravableCompra).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.RentaCompra).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.FonaviCompra).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.ServicioCompra).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);
            builder.Property(x => x.IgvExteriorCompra).IsRequired().HasColumnType("decimal(12,2)").HasDefaultValue(0);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Índices para la búsqueda paginada (proveedor, estado, fecha de proceso).
            builder.HasIndex(x => x.ProveedorCode);
            builder.HasIndex(x => x.Estado);
            builder.HasIndex(x => x.CreatedAt);

            builder.HasMany(x => x.Detalles)
                .WithOne(x => x.OrdenCompraAdministracion)
                .HasForeignKey(x => x.OrdenCompraAdministracionCode)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
