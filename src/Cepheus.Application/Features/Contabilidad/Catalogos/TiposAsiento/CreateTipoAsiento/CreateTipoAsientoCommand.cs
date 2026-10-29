using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.CreateTipoAsiento
{
    public record CreateTipoAsientoCommand(
        string Name,
        TipoCambioAplicable ExchangeRateType,
        ValorizacionAsiento Valuation
    ) : IRequest<TipoAsientoResponse>;
}
