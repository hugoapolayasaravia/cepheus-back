// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion.OrdenCompraAdministracionDetalles/CreateOrdenCompraAdministracionDetalle/CreateOrdenCompraAdministracionDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.CreateOrdenCompraAdmDetalle
{
    public record CreateOrdenCompraAdmDetalleCommand(
        string OrdenCompraAdministracionCode,
        string Glosa,
        string UnidadMedidaCode,
        decimal Cantidad,
        decimal Precio,
        decimal Descuento,
        string SubCentroCostoCode
    ) : IRequest<OrdenCompraAdmResponse>, IOrdenCompraAdmLinea;
}
