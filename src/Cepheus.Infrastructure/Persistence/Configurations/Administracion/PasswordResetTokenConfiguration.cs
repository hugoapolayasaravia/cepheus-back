using Cepheus.Domain.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Administracion
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.ToTable("PasswordResetTokens", schema: "admin");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            // IsActive es una propiedad calculada en memoria, no se mapea a columna
            builder.Ignore(x => x.IsActive);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // Sin propiedad de navegación en User: no hace falta tocar User.cs
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
