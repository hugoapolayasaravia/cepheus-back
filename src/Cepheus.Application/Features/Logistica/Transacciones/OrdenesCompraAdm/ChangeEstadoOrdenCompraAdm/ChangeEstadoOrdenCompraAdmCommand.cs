// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/ChangeEstadoOrdenCompraAdministracion/ChangeEstadoOrdenCompraAdministracionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.ChangeEstadoOrdenCompraAdm
{
    public record ChangeEstadoOrdenCompraAdmCommand(string Code, string NuevoEstado)
        : IRequest<OrdenCompraAdmResponse>;
}
