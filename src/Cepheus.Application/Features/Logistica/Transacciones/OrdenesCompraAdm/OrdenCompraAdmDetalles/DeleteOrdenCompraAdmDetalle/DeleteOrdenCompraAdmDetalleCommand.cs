// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion.OrdenCompraAdministracionDetalles/DeleteOrdenCompraAdministracionDetalle/DeleteOrdenCompraAdministracionDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.DeleteOrdenCompraAdmDetalle
{
    public record DeleteOrdenCompraAdmDetalleCommand(string OrdenCompraAdministracionCode, int ItemNumber)
        : IRequest<OrdenCompraAdmResponse>;
}
