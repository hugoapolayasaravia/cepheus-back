// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion.OrdenCompraAdministracionDetalles/UpdateOrdenCompraAdministracionDetalle/UpdateOrdenCompraAdministracionDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.UpdateOrdenCompraAdmDetalle
{
    public record UpdateOrdenCompraAdmDetalleCommand(
        string OrdenCompraAdministracionCode,
        int ItemNumber,
        string Glosa,
        string UnidadMedidaCode,
        decimal Cantidad,
        decimal Precio,
        decimal Descuento,
        string SubCentroCostoCode
    ) : IRequest<OrdenCompraAdmResponse>, IOrdenCompraAdmLinea;
}
