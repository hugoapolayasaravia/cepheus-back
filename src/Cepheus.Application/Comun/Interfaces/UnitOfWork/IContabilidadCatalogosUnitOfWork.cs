using Cepheus.Domain.Contabilidad.Catalogos;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IContabilidadCatalogosUnitOfWork
    {
        IRepository<TipoAnexo> TiposAnexo { get; }
        IRepository<TipoCuentaContable> TiposCuentaContable { get; }
        IRepository<TipoAsiento> TiposAsiento { get; }
    }
}