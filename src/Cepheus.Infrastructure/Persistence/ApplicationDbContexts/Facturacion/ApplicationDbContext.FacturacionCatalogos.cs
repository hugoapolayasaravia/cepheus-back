using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Facturacion - Catalogos

    public DbSet<TipoCliente> TiposCliente => Set<TipoCliente>();

    public DbSet<ClasificacionCliente> ClasificacionesCliente => Set<ClasificacionCliente>();
    public DbSet<SegmentoVentas> SegmentosVentas => Set<SegmentoVentas>();
    public DbSet<AnalisisVenta> AnalisisVentas => Set<AnalisisVenta>();
    public DbSet<FormaPagoVenta> FormasPagoVenta => Set<FormaPagoVenta>();
    public DbSet<CategoriaProducto> CategoriasProducto => Set<CategoriaProducto>();
    public DbSet<TipoBien> TiposBien => Set<TipoBien>();
    public DbSet<TipoOperacion> TiposOperacion => Set<TipoOperacion>();
    public DbSet<AtributoConcreto> AtributosConcreto => Set<AtributoConcreto>();
    public DbSet<TipoProducto> TiposProducto => Set<TipoProducto>();
    public DbSet<UnidadMedidaVenta> UnidadesMedidaVenta => Set<UnidadMedidaVenta>();
    public DbSet<ListaPrecio> ListasPrecio => Set<ListaPrecio>();
    public DbSet<StockProducto> StockProductos => Set<StockProducto>();
    public DbSet<Flete> Fletes => Set<Flete>();
    public DbSet<AlturaLosa> AlturasLosa => Set<AlturaLosa>();
    public DbSet<PrecioProducto> PreciosProducto => Set<PrecioProducto>();
    public DbSet<NotaCotizacionPlantilla> NotasCotizacionPlantilla => Set<NotaCotizacionPlantilla>();

    public DbSet<ResponsableObra> ResponsablesObra => Set<ResponsableObra>();

    public DbSet<TipoHojaCorte> TiposHojaCorte => Set<TipoHojaCorte>();
    public DbSet<MotivoSolicitudNotaCredito> MotivosSolicitudNotaCredito => Set<MotivoSolicitudNotaCredito>();

    public DbSet<ElementoConcreto> ElementosConcreto => Set<ElementoConcreto>();

    public DbSet<MotivoChequeRebote> MotivosChequeRebote => Set<MotivoChequeRebote>();
}