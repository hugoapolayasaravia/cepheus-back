using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.GetCuentasBancariasPaginated
{
    public class GetCuentasBancariasPaginatedQuery : PagedRequest, IRequest<PagedResult<CuentaBancariaResponse>>
    {
        public string? Search { get; set; }
        public string? BankCode { get; set; }
        public string? CurrencyCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
