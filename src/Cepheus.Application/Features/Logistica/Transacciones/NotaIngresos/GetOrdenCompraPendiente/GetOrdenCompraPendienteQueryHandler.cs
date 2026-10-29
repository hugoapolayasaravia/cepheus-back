using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetOrdenCompraPendiente;

public sealed class GetOrdenCompraPendienteQueryHandler
    : IRequestHandler<GetOrdenCompraPendienteQuery, OrdenCompraPendienteResponse>
{
    private readonly IUnitOfWork _uow;

    public GetOrdenCompraPendienteQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenCompraPendienteResponse> Handle(
        GetOrdenCompraPendienteQuery request, CancellationToken ct)
    {
        var planta = NotaIngresoRules.Normalize(request.PlantaCode);
        var code = NotaIngresoRules.Normalize(request.OrdenCompraCode);

        var oc = await _uow.Logistica.Transacciones.OrdenesCompra.Query()
            .AsNoTracking()
            .Include(x => x.Proveedor)
            .Include(x => x.FormaPago)
            .Include(x => x.Detalles).ThenInclude(d => d.Articulo)
            .Include(x => x.Detalles).ThenInclude(d => d.Origenes)
            .FirstOrDefaultAsync(x => x.PlantaCode == planta && x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Orden de Compra {code} no existe en la planta {planta}.");

        NotaIngresoOrdenCompraUpdater.EnsureReceivable(oc);

        var response = new OrdenCompraPendienteResponse
        {
            PlantaCode = oc.PlantaCode,
            OrdenCompraCode = oc.Code,
            Estado = oc.Estado,
            ProveedorCode = oc.ProveedorCode,
            ProveedorName = oc.Proveedor.LegalName,
            MonedaCode = oc.MonedaCode,
            FormaPagoCode = oc.FormaPagoCode,
            FormaPagoName = oc.FormaPago.Name
        };

        foreach (var detalle in oc.Detalles.OrderBy(d => d.ItemNumber))
        {
            var bruto = detalle.CantidadArticulo * detalle.PrecioArticulo;
            var descuentoPct = bruto > 0
                ? Math.Round(detalle.DescuentoArticulo / bruto * 100m, 2, MidpointRounding.AwayFromZero)
                : 0m;

            var pedidos = detalle.Origenes.Count == 0
                ? new List<string?> { null }
                : detalle.Origenes.Select(o => (string?)o.PedidoCode).Distinct().ToList();

            foreach (var pedido in pedidos)
            {
                var pendiente = NotaIngresoOrdenCompraUpdater.Pending(detalle, pedido);
                if (pendiente <= 0) continue;

                response.Lineas.Add(new OrdenCompraPendienteLineaResponse
                {
                    ArticuloCode = detalle.ArticuloCode,
                    ArticuloName = detalle.Articulo.Name,
                    UnidadMedidaCode = detalle.Articulo.UnidadMedidaCode,
                    PedidoCode = pedido,
                    CantidadPendiente = pendiente,
                    Precio = detalle.PrecioArticulo,
                    DescuentoPorcentaje = descuentoPct
                });
            }
        }

        return response;
    }
}
