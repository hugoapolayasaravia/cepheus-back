// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/GetOrdenCompraAdministracionByCode/GetOrdenCompraAdministracionByCodeQuery.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.GetOrdenCompraAdmByCode
{
    public record GetOrdenCompraAdmByCodeQuery(string Code) : IRequest<OrdenCompraAdmResponse>;
}
