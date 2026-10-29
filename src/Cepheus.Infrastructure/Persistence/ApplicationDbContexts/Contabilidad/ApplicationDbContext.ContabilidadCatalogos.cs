using Cepheus.Domain.Contabilidad.Catalogos;
using Cepheus.Domain.Mantenimiento.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Contabilidad - Catalogos

    public DbSet<TipoAnexo> TiposAnexo => Set<TipoAnexo>();
    public DbSet<TipoCuentaContable> TiposCuentaContable => Set<TipoCuentaContable>();
    public DbSet<TipoAsiento> TiposAsiento => Set<TipoAsiento>();

}