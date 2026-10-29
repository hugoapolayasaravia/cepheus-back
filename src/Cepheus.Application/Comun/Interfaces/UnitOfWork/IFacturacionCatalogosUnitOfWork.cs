using Cepheus.Domain.Facturacion.Catalogos;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IFacturacionCatalogosUnitOfWork
    {

        IRepository<TipoCliente> TiposCliente { get; }
        IRepository<ClasificacionCliente> ClasificacionesCliente { get; }
        IRepository<SegmentoVentas> SegmentosVentas { get; }
        IRepository<AnalisisVenta> AnalisisVentas { get; }
        IRepository<TipoValorizacion> TiposValorizacion { get; }
        IRepository<FormaPagoVenta> FormasPagoVenta { get; }
        IRepository<CategoriaProducto> CategoriasProducto { get; }
        IRepository<TipoBien> TiposBien { get; }
        IRepository<TipoOperacion> TiposOperacion { get; }
        IRepository<AtributoConcreto> AtributosConcreto { get; }
        IRepository<TipoProducto> TiposProducto { get; }
        IRepository<UnidadMedidaVenta> UnidadesMedidaVenta { get; }
        IRepository<ListaPrecio> ListasPrecio { get; }
        IRepository<StockProducto> StockProductos { get; }

        IRepository<Flete> Fletes { get; }
        IRepository<AlturaLosa> AlturasLosa { get; }
        IRepository<PrecioProducto> PreciosProducto { get; }
        IRepository<NotaCotizacionPlantilla> NotasCotizacionPlantilla { get; }
        IRepository<ResponsableObra> ResponsablesObra { get; }

        IRepository<TipoHojaCorte> TiposHojaCorte { get; }
        IRepository<MotivoSolicitudNotaCredito> MotivosSolicitudNotaCredito { get; }

        IRepository<ElementoConcreto> ElementosConcreto { get; }

        IRepository<MotivoChequeRebote> MotivosChequeRebote { get; }
    }
}