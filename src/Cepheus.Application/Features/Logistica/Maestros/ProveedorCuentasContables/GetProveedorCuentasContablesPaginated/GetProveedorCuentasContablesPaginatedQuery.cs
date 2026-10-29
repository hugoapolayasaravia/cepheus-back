using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.GetProveedorCuentasContablesPaginated
{
    public class GetProveedorCuentasContablesPaginatedQuery
        : PagedRequest, IRequest<PagedResult<ProveedorCuentaContableResponse>>
    {
        public string? ProveedorCode { get; set; }
        public string? MonedaCode { get; set; }
        public string? AccountingAccountCode { get; set; }
    }
}
