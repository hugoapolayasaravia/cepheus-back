using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("Productos", schema: "facturacion");

            builder.HasKey(x => new { x.TipoProductoCode, x.Code });

            builder.Ignore(x => x.FullCode);

            builder.Property(x => x.TipoProductoCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(4)");

            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Property(x => x.ShortName).HasMaxLength(255);

            builder.Property(x => x.UnitCode).IsRequired().HasMaxLength(5);

            builder.Property(x => x.AccountingAccountCode).HasMaxLength(8);
            builder.Property(x => x.TransportAccountCode).HasMaxLength(8);
            builder.Property(x => x.CreditNoteAccountCode).HasMaxLength(8);

            builder.Property(x => x.LengthLimit).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.TransportTipoProductoCode).HasMaxLength(2);
            builder.Property(x => x.TransportCode).HasMaxLength(4);
            builder.Property(x => x.CategoryCode).HasMaxLength(3);
            builder.Property(x => x.GoodsTypeCode).HasMaxLength(3);
            builder.Property(x => x.OperationTypeCode).HasMaxLength(2);

            builder.Property(x => x.IsPumpable).IsRequired();
            builder.Property(x => x.IsSubjectToDetraction).IsRequired();
            builder.Property(x => x.CementValue).HasColumnType("decimal(18,2)");

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => x.CategoryCode);

            builder.HasOne(x => x.TipoProducto)
                .WithMany()
                .HasForeignKey(x => x.TipoProductoCode)
                .HasPrincipalKey(t => t.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Unit)
                .WithMany()
                .HasForeignKey(x => x.UnitCode)
                .HasPrincipalKey(u => u.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.GoodsType)
                .WithMany()
                .HasForeignKey(x => x.GoodsTypeCode)
                .HasPrincipalKey(g => g.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OperationType)
                .WithMany()
                .HasForeignKey(x => x.OperationTypeCode)
                .HasPrincipalKey(o => o.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // Producto de transporte: FK compuesta opcional a la propia tabla
            builder.HasOne(x => x.TransportProducto)
                .WithMany()
                .HasForeignKey(x => new { x.TransportTipoProductoCode, x.TransportCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);

            // Especificación técnica del concreto -> catálogo AtributoConcreto
            builder.Property(x => x.StrengthCode).HasMaxLength(4);
            builder.HasOne<AtributoConcreto>()
                .WithMany()
                .HasForeignKey(x => x.StrengthCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CementTypeCode).HasMaxLength(4);
            builder.HasOne<AtributoConcreto>()
                .WithMany()
                .HasForeignKey(x => x.CementTypeCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.StoneSizeCode).HasMaxLength(4);
            builder.HasOne<AtributoConcreto>()
                .WithMany()
                .HasForeignKey(x => x.StoneSizeCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.SlumpCode).HasMaxLength(4);
            builder.HasOne<AtributoConcreto>()
                .WithMany()
                .HasForeignKey(x => x.SlumpCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.WaterCementRatioCode).HasMaxLength(4);
            builder.HasOne<AtributoConcreto>()
                .WithMany()
                .HasForeignKey(x => x.WaterCementRatioCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AgeCode).HasMaxLength(4);
            builder.HasOne<AtributoConcreto>()
                .WithMany()
                .HasForeignKey(x => x.AgeCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.SpecialConditionCode).HasMaxLength(4);
            builder.HasOne<AtributoConcreto>()
                .WithMany()
                .HasForeignKey(x => x.SpecialConditionCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.MixProportionCode).HasMaxLength(4);
            builder.HasOne<AtributoConcreto>()
                .WithMany()
                .HasForeignKey(x => x.MixProportionCode)
                .HasPrincipalKey(a => a.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<CuentaContable>()
                .WithMany()
                .HasForeignKey(x => x.AccountingAccountCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<CuentaContable>()
                .WithMany()
                .HasForeignKey(x => x.TransportAccountCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<CuentaContable>()
                .WithMany()
                .HasForeignKey(x => x.CreditNoteAccountCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
