using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface ILogisticaCatalogosUnitOfWork
    {
        IRepository<Familia> Familias { get; }
        IRepository<SubFamilia> SubFamilias { get; }
        IRepository<UnidadMedida> UnidadesMedida { get; }
        IRepository<TipoCompra> TiposCompra { get; }
        IRepository<NotaCompra> NotasCompra { get; }
        IRepository<LugarEnvio> LugaresEnvio { get; }
        IRepository<Comprador> Compradores { get; }
        IRepository<Tramite> Tramites { get; }
        IRepository<TipoPedido> TiposPedido { get; }
        IRepository<UnidadNegocio> UnidadesNegocio { get; }
        IRepository<TipoVale> TiposVale { get; }
        IRepository<TipoArticulo> TiposArticulo { get; }
        IRepository<PlanArticulo> PlanesArticulo { get; }
        IRepository<FormaPago> FormasPago { get; }
        IRepository<Nivel> Niveles { get; }
        IRepository<TipoTransaccion> TiposTransaccion { get; }
        IRepository<RangoAprobacion> RangosAprobacion { get; }
        IRepository<AprobadorAsignado> AprobadoresAsignados { get; }
        IRepository<MotivoDevolucionArticulo> MotivosDevolucionArticulo { get; }
        IRepository<TipoValeArticulo> TiposValeArticulo { get; }

        IRepository<UsuarioPlanta> UsuariosPlanta { get; }

    }
}
