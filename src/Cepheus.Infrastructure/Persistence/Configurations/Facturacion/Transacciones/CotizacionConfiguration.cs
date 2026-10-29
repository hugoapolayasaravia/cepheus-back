using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class CotizacionConfiguration : IEntityTypeConfiguration<Cotizacion>
    {
        public void Configure(EntityTypeBuilder<Cotizacion> builder)
        {
            builder.ToTable("Cotizaciones", schema: "facturacion");

            builder.HasKey(x => new { x.NegocioCode, x.Year, x.Month, x.Code });

            builder.Property(x => x.NegocioCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Month).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");

            builder.Property(x => x.VendedorCode).IsRequired().HasMaxLength(4);
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.CurrencyCode).IsRequired().HasColumnType("char(1)");
            builder.Property(x => x.FormaPagoVentaCode).IsRequired().HasMaxLength(2);
            builder.Property(x => x.TecnicoCode).HasMaxLength(5);

            builder.Property(x => x.AppliesIgv).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.Discount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.GlobalVolume).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.IsEditable).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.Type).IsRequired().HasConversion<int>().HasDefaultValue(TipoCotizacion.Cotizacion).HasSentinel(default(TipoCotizacion));
            builder.Property(x => x.MetradoCalculationSystem).HasConversion<int?>();
            builder.Property(x => x.WorkDurationMonths).IsRequired().HasDefaultValue(0);

            builder.Property(x => x.ClienteCode).HasMaxLength(5);
            builder.Property(x => x.Ruc).HasMaxLength(11);
            builder.Property(x => x.ClientName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.ClientAddress).HasMaxLength(180);
            builder.Property(x => x.ClientAddressUbigeoCode).IsRequired().HasMaxLength(6).HasDefaultValue("150101");

            builder.Property(x => x.ObraCode).HasMaxLength(3);
            builder.Property(x => x.WorkName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.ProjectStatus).IsRequired().HasConversion<int>();
            builder.Property(x => x.WorkAddressUbigeoCode).IsRequired().HasMaxLength(6).HasDefaultValue("150101");
            builder.Property(x => x.WorkAddress).IsRequired().HasMaxLength(150);

            builder.Property(x => x.ContactName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.ContactPhone).IsRequired().HasMaxLength(150);
            builder.Property(x => x.ContactEmail).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Reference).HasMaxLength(50);

            builder.Property(x => x.FleteCode).IsRequired().HasMaxLength(2).HasDefaultValue("01");
            builder.Property(x => x.IgvRate).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.ProcessDate).IsRequired();

            builder.Property(x => x.GrossAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.IgvAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.NetAmount).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.Status).IsRequired().HasConversion<int>().HasDefaultValue(EstadoCotizacion.Pendiente).HasSentinel(default(EstadoCotizacion));

            builder.Property(x => x.OriginNegocioCode).HasMaxLength(2);
            builder.Property(x => x.OriginYear).HasMaxLength(4);
            builder.Property(x => x.OriginMonth).HasMaxLength(2);
            builder.Property(x => x.OriginCode).HasMaxLength(8);

            builder.Property(x => x.ApprovedBy).HasMaxLength(20);
            builder.Property(x => x.CancelReason).HasMaxLength(500);
            builder.Property(x => x.CanceledBy).HasMaxLength(20);

            builder.Property(x => x.IsPrinted).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Observations).HasMaxLength(500);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Relaciones
            builder.HasOne(x => x.Negocio).WithMany().HasForeignKey(x => x.NegocioCode)
                .HasPrincipalKey(n => n.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Vendedor).WithMany().HasForeignKey(x => x.VendedorCode)
                .HasPrincipalKey(v => v.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FormaPagoVenta).WithMany().HasForeignKey(x => x.FormaPagoVentaCode)
                .HasPrincipalKey(f => f.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Tecnico).WithMany().HasForeignKey(x => x.TecnicoCode)
                .HasPrincipalKey(t => t.TrabajadorCode).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ClientAddressUbigeo).WithMany().HasForeignKey(x => x.ClientAddressUbigeoCode)
                .HasPrincipalKey(u => u.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.WorkAddressUbigeo).WithMany().HasForeignKey(x => x.WorkAddressUbigeoCode)
                .HasPrincipalKey(u => u.Code).OnDelete(DeleteBehavior.Restrict);

            // FK compuesta opcional (ClienteCode, ObraCode) -> Obra: solo se valida
            // cuando ambas columnas tienen valor.
            builder.HasOne(x => x.Obra).WithMany().HasForeignKey(x => new { x.ClienteCode, x.ObraCode })
                .HasPrincipalKey(o => new { o.ClienteCode, o.Code }).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Flete).WithMany().HasForeignKey(x => x.FleteCode)
                .HasPrincipalKey(f => f.Code).OnDelete(DeleteBehavior.Restrict);

            // Auto-FK compuesta opcional: cotización origen (copia/recotización)
            builder.HasOne(x => x.Origin).WithMany()
                .HasForeignKey(x => new { x.OriginNegocioCode, x.OriginYear, x.OriginMonth, x.OriginCode })
                .HasPrincipalKey(x => new { x.NegocioCode, x.Year, x.Month, x.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ClienteCode);
            builder.HasIndex(x => x.VendedorCode);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.Date);
        }
    }
}
