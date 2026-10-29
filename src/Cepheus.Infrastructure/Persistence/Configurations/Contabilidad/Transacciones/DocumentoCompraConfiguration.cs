using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class DocumentoCompraConfiguration : IEntityTypeConfiguration<DocumentoCompra>
    {
        public void Configure(EntityTypeBuilder<DocumentoCompra> builder)
        {
            builder.ToTable("DocumentosCompra", schema: "contabilidad");

            builder.HasKey(x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode });

            builder.Property(x => x.ComprobantePagoCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.ComprobantePago).WithMany().HasForeignKey(x => x.ComprobantePagoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.NumeroDocumento).IsRequired().HasColumnType("char(15)");

            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.FechaEmision).IsRequired().HasColumnType("date");
            builder.Property(x => x.FechaRecepcion).IsRequired().HasColumnType("date");
            builder.Property(x => x.FechaDocumento).HasColumnType("date");
            builder.Property(x => x.FechaVencimiento).IsRequired().HasColumnType("date");

            builder.Property(x => x.FormaPagoCode).IsRequired().HasMaxLength(2);
            builder.HasOne(x => x.FormaPago).WithMany().HasForeignKey(x => x.FormaPagoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.MonedaCode).IsRequired().HasMaxLength(3);
            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .OnDelete(DeleteBehavior.Restrict);

            // Solo la fecha: el valor del tipo de cambio se consulta en Comunes.TiposCambio (Date único).
            builder.Property(x => x.TipoCambioFecha).HasColumnType("date");

            builder.Property(x => x.ParcialSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.IgvSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.NoGravableSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.RentaSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.FonaviSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.ServicioSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.IgvExteriorSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.TotalSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.PagadoSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);

            // Columna calculada (legacy: PendienteS_dco AS totals_dco - pagados_dco).
            builder.Property(x => x.PendienteSol)
                .HasColumnType("decimal(24,2)")
                .HasComputedColumnSql("[TotalSol] - [PagadoSol]");

            builder.Property(x => x.Glosa).IsRequired().HasMaxLength(50);

            builder.Property(x => x.Origen).IsRequired().HasConversion<int>();
            builder.Property(x => x.Procedencia).HasConversion<int>();
            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();

            // Detracciones
            builder.Property(x => x.TipoBienCode).HasMaxLength(3);
            builder.HasOne(x => x.TipoBien).WithMany().HasForeignKey(x => x.TipoBienCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.TipoOperacionCode).HasMaxLength(2);
            builder.HasOne(x => x.TipoOperacion).WithMany().HasForeignKey(x => x.TipoOperacionCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.OperacionDetraccion).HasColumnType("char(10)");
            builder.Property(x => x.ConstanciaDetraccion).HasColumnType("char(10)");
            builder.Property(x => x.FechaConstanciaDetraccion).HasColumnType("date");
            builder.Property(x => x.NumeroDetraccion).HasColumnType("char(10)");
            builder.Property(x => x.FechaEmisionDetraccion).HasColumnType("date");
            builder.Property(x => x.MontoDetraccionSol).IsRequired().HasColumnType("decimal(16,2)").HasDefaultValue(0);

            // Documento origen (nota de crédito -> factura): FK compuesta autorreferencia, mismo proveedor.
            builder.Property(x => x.DocumentoOrigenComprobantePagoCode).HasColumnType("char(2)");
            builder.Property(x => x.DocumentoOrigenNumero).HasColumnType("char(15)");
            builder.HasOne(x => x.DocumentoOrigen).WithMany()
                .HasForeignKey(x => new { x.DocumentoOrigenComprobantePagoCode, x.DocumentoOrigenNumero, x.ProveedorCode })
                .HasPrincipalKey(x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.OrdenCompraAdministracionCode).HasColumnType("char(6)");
            builder.HasOne(x => x.OrdenCompraAdministracion).WithMany()
                .HasForeignKey(x => x.OrdenCompraAdministracionCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.FechaCancelacion).HasColumnType("date");
            builder.Property(x => x.AsientoProvisionCode).HasColumnType("char(13)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Índices para la búsqueda paginada.
            builder.HasIndex(x => x.ProveedorCode);
            builder.HasIndex(x => x.Estado);
            builder.HasIndex(x => x.FechaEmision);
            builder.HasIndex(x => x.OrdenCompraAdministracionCode);

            builder.HasMany(x => x.Detalles)
                .WithOne(x => x.DocumentoCompra)
                .HasForeignKey(x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
