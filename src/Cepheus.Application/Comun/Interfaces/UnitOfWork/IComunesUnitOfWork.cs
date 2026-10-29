using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IComunesUnitOfWork
    {
        IRepository<Planta> Plantas { get; }
        IRepository<Moneda> Monedas { get; }
        IRepository<TipoDocumento> TiposDocumento { get; }
        IRepository<ComprobantePago> ComprobantesPago { get; }
        IRepository<Ubigeo> Ubigeos { get; }
        IRepository<TipoCambio> TiposCambio { get; }
        IRepository<ControlVentas> ControlesVentas { get; }
        IRepository<MotivoDevolucion> MotivosDevolucion { get; }
        IRepository<Banco> Bancos { get; }
        IRepository<Negocio> Negocios { get; }
        IRepository<PeriodoIgv> PeriodosIgv { get; }
    }
}
