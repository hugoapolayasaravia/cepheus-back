using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.GetCuentaContableByCode
{
    public record GetCuentaContableByCodeQuery(string Code) : IRequest<CuentaContableResponse>;
}
