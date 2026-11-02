using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class PagoProveedorConfiguration : IEntityTypeConfiguration<PagoProveedor>
    {
        public void Configure(EntityTypeBuilder<PagoProveedor> builder)
        {
            builder.ToTable("PagosProveedor", schema: "contabilidad");

            builder.HasKey(x => x.Code);
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(10)");

            builder.Property(x => x.NumeroCheque).IsRequired().HasMaxLength(10);
            builder.Property(x => x.NumeroControl).IsRequired().HasColumnType("char(1)").HasDefaultValue("*");

            // Banco y cuenta bancaria (FK compuesta a CuentaBancaria).
            builder.Property(x => x.BancoCode).IsRequired().HasColumnType("char(3)");
            builder.HasOne(x => x.Banco).WithMany().HasForeignKey(x => x.BancoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CuentaBancariaNumero).IsRequired().HasColumnType("char(20)");
            builder.HasOne(x => x.CuentaBancaria).WithMany()
                .HasForeignKey(x => new { x.BancoCode, x.CuentaBancariaNumero })
                .HasPrincipalKey(x => new { x.BankCode, x.AccountNumber })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.NumeroComprobanteRetencion).HasColumnType("char(10)");
            builder.Property(x => x.FechaComprobanteRetencion).HasColumnType("date");

            builder.Property(x => x.FechaEmision).IsRequired().HasColumnType("date");
            builder.Property(x => x.FechaAnulacion).HasColumnType("date");

            builder.Property(x => x.MonedaCode).IsRequired().HasMaxLength(3);
            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.PagoSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.RetencionSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.TotalSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.PagoRealSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.PagoSolVenta).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);
            builder.Property(x => x.MontoReferencial).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);

            builder.Property(x => x.AsientoCancelacionCode).HasColumnType("char(13)");
            builder.Property(x => x.AsientoAnulacionCode).HasColumnType("char(13)");

            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();
            builder.Property(x => x.Tipo).IsRequired().HasConversion<int>();

            builder.Property(x => x.ComprobanteCajaBancosImpreso).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.ComprobanteRetencionImpreso).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Afecto).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.Detraccion).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Canje).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.DepositoElectronico).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.Glosa).IsRequired().HasMaxLength(200);
            builder.Property(x => x.PagarA).IsRequired().HasMaxLength(150);

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.Planta).WithMany().HasForeignKey(x => x.PlantaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CuentaContableCode).IsRequired().HasColumnType("char(8)");
            builder.HasOne(x => x.CuentaContable).WithMany().HasForeignKey(x => x.CuentaContableCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ClienteCode).HasColumnType("char(5)");
            builder.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.FormaPagoCancelacionCode).HasColumnType("char(1)");
            builder.HasOne(x => x.FormaPagoCancelacion).WithMany().HasForeignKey(x => x.FormaPagoCancelacionCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ProveedorCuenta).WithMany().HasForeignKey(x => x.ProveedorCuentaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Índices para la búsqueda paginada.
            builder.HasIndex(x => x.ProveedorCode);
            builder.HasIndex(x => x.Estado);
            builder.HasIndex(x => x.FechaEmision);
            builder.HasIndex(x => new { x.BancoCode, x.NumeroCheque });

            builder.HasMany(x => x.Detalles)
                .WithOne(x => x.Pago)
                .HasForeignKey(x => x.PagoCode)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
