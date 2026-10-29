using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class HojaCorteConfiguration : IEntityTypeConfiguration<HojaCorte>
    {
        public void Configure(EntityTypeBuilder<HojaCorte> builder)
        {
            builder.ToTable("HojasCorte", schema: "facturacion");

            // Clave primaria compuesta: el correlativo se genera por año.
            builder.HasKey(x => new { x.Year, x.Code });

            builder.Property(x => x.Year)
                .IsRequired()
                .HasColumnType("char(4)");

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(8)");

            // Tipo de hoja de corte.
            builder.Property(x => x.TipoHojaCorteCode)
                .IsRequired()
                .HasMaxLength(2);

            // Cliente y datos históricos de la obra.
            builder.Property(x => x.ClienteCode)
                .HasMaxLength(5);

            builder.Property(x => x.ClientName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.ObraCode)
                .HasMaxLength(3);

            builder.Property(x => x.WorkName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.WorkAddress)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.WorkAddressUbigeoCode)
                .IsRequired()
                .HasMaxLength(6);

            // Responsable comercial y técnico.
            builder.Property(x => x.TecnicoCode)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(x => x.VendedorCode)
                .IsRequired()
                .HasMaxLength(4);

            // Datos de ubicación de la obra.
            builder.Property(x => x.Sector)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Floor)
                .IsRequired()
                .HasMaxLength(30);

            // Fechas.
            builder.Property(x => x.Date)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.ViguetaDispatchDate)
                .HasColumnType("date");

            builder.Property(x => x.BovedillaDispatchDate)
                .HasColumnType("date");

            builder.Property(x => x.ClientViguetaDispatchDate);

            builder.Property(x => x.ClientBovedillaDispatchDate);

            builder.Property(x => x.StorageDate)
                .HasColumnType("date");

            // Orden de trabajo de ventas (opcional): FK real a facturacion.OrdenesTrabajoVenta.
            builder.Property(x => x.OrdenTrabajoCode)
                .HasColumnType("char(8)");

            builder.HasOne(x => x.OrdenTrabajo)
                .WithMany()
                .HasForeignKey(x => x.OrdenTrabajoCode)
                .HasPrincipalKey(o => o.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // Estado y observaciones.
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.Observations)
                .HasMaxLength(500);

            // Programación.
            builder.Property(x => x.IsScheduled)
                .IsRequired()
                .HasDefaultValue(false);

            // Relación opcional con la hoja de corte padre.
            builder.Property(x => x.ParentYear)
                .HasColumnType("char(4)");

            builder.Property(x => x.ParentCode)
                .HasColumnType("char(8)");

            builder.Property(x => x.Turn)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.IsModifiable)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.Motivo)
                .HasConversion<int?>();

            builder.Property(x => x.ProductType)
                .HasConversion<int?>();

            // Auditoría.
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            // Concurrencia optimista.
            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // Relaciones con catálogos y maestros.
            builder.HasOne(x => x.TipoHojaCorte)
                .WithMany()
                .HasForeignKey(x => x.TipoHojaCorteCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cliente)
                .WithMany()
                .HasForeignKey(x => x.ClienteCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.WorkAddressUbigeo)
                .WithMany()
                .HasForeignKey(x => x.WorkAddressUbigeoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Tecnico)
                .WithMany()
                .HasForeignKey(x => x.TecnicoCode)
                .HasPrincipalKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Vendedor)
                .WithMany()
                .HasForeignKey(x => x.VendedorCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación opcional: ClienteCode + ObraCode -> Obra.
            builder.HasOne(x => x.Obra)
                .WithMany()
                .HasForeignKey(x => new { x.ClienteCode, x.ObraCode })
                .HasPrincipalKey(x => new { x.ClienteCode, x.Code })
                .OnDelete(DeleteBehavior.Restrict);

            // Auto-FK compuesta opcional: hoja de corte padre.
            builder.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => new { x.ParentYear, x.ParentCode })
                .HasPrincipalKey(x => new { x.Year, x.Code })
                .OnDelete(DeleteBehavior.Restrict);

            // Índices para búsquedas frecuentes.
            builder.HasIndex(x => x.ClienteCode);

            builder.HasIndex(x => x.VendedorCode);

            builder.HasIndex(x => x.TecnicoCode);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.Date);

            builder.HasIndex(x => x.OrdenTrabajoCode);
        }
    }
}
