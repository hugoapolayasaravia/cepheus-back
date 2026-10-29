using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public sealed class OrdenServicioConfiguration : IEntityTypeConfiguration<OrdenServicio>
{
    public void Configure(EntityTypeBuilder<OrdenServicio> builder)
    {
        builder.ToTable("OrdenesServicio", "logistica");

        builder.HasKey(x => new { x.PlantaCode, x.Code });

        builder.Property(x => x.PlantaCode).HasColumnType("char(2)").IsRequired();
        builder.Property(x => x.Code).HasColumnType("char(6)").IsRequired();

        builder.HasOne(x => x.Planta)
            .WithMany()
            .HasForeignKey(x => x.PlantaCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Enum persistido como int con el valor del código legacy (mismo criterio que NotaIngreso).
        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();

        builder.Property(x => x.ComprobantePagoCode).IsRequired().HasColumnType("char(2)").IsFixedLength();
        builder.HasOne(x => x.ComprobantePago)
            .WithMany()
            .HasForeignKey(x => x.ComprobantePagoCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.NumeroDocumento).HasMaxLength(15);

        builder.Property(x => x.ProveedorCode).HasColumnType("char(5)").IsRequired();
        builder.HasOne(x => x.Proveedor)
            .WithMany()
            .HasForeignKey(x => x.ProveedorCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.MonedaCode).IsRequired().HasMaxLength(3);
        builder.HasOne(x => x.Moneda)
            .WithMany()
            .HasForeignKey(x => x.MonedaCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.FormaPagoCode).IsRequired().HasMaxLength(2);
        builder.HasOne(x => x.FormaPago)
            .WithMany()
            .HasForeignKey(x => x.FormaPagoCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.FechaRecepcion).IsRequired();
        builder.Property(x => x.FechaEmision).IsRequired();

        builder.Property(x => x.TipoCambio).HasColumnType("decimal(12,4)").IsRequired();
        builder.Property(x => x.Igv).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Total).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Monto).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.NoGravable).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Renta).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Fonavi).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Servicio).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.IgvExterior).HasColumnType("decimal(12,2)").IsRequired();

        builder.Property(x => x.AsientoContable).HasMaxLength(13);

        builder.Property(x => x.AprobadoPor).HasMaxLength(250);
        builder.Property(x => x.ProcesadoPor).HasMaxLength(250);

        builder.Property(x => x.CompradorCode).HasColumnType("char(3)").IsRequired();
        builder.HasOne(x => x.Comprador)
            .WithMany()
            .HasForeignKey(x => x.CompradorCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.LugarEnvioCode).HasColumnType("char(3)").IsRequired();
        builder.HasOne(x => x.LugarEnvio)
            .WithMany()
            .HasForeignKey(x => x.LugarEnvioCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TramiteCode).HasColumnType("char(1)").IsRequired();
        builder.HasOne(x => x.Tramite)
            .WithMany()
            .HasForeignKey(x => x.TramiteCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.NotaCompraCode).HasColumnType("char(3)");
        builder.HasOne(x => x.NotaCompra)
            .WithMany()
            .HasForeignKey(x => x.NotaCompraCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UnidadNegocioCode).HasColumnType("char(6)").IsRequired();
        builder.HasOne(x => x.UnidadNegocio)
            .WithMany()
            .HasForeignKey(x => x.UnidadNegocioCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Vale de salida asociado (Codigo_Val). Mismo PlantaCode + código del vale.
        builder.Property(x => x.ValeSalidaCode).HasColumnType("char(6)").IsRequired();
        builder.HasOne(x => x.ValeSalida)
            .WithMany()
            .HasForeignKey(x => new { x.PlantaCode, x.ValeSalidaCode })
            .HasPrincipalKey(x => new { x.PlantaCode, x.Code })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.PlantaCode, x.ValeSalidaCode }).IsUnique();

        builder.Property(x => x.Observaciones1).HasMaxLength(200);
        builder.Property(x => x.Observaciones2).HasMaxLength(200);

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();

        // Listado legacy (Logi_sp_Listado_MOrdenServicio_I) y control de documento duplicado.
        builder.HasIndex(x => new { x.PlantaCode, x.CreatedAt });
        builder.HasIndex(x => new { x.ComprobantePagoCode, x.ProveedorCode, x.NumeroDocumento });

        builder.HasMany(x => x.Detalles)
            .WithOne(x => x.OrdenServicio)
            .HasForeignKey(x => new { x.PlantaCode, x.OrdenServicioCode })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
