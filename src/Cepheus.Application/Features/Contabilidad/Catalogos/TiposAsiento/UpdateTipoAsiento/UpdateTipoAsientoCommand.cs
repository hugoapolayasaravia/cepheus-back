using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.UpdateTipoAsiento
{
    public record UpdateTipoAsientoCommand(
        string Code,
        string Name,
        TipoCambioAplicable ExchangeRateType,
        ValorizacionAsiento Valuation,
        byte[] RowVersion
    ) : IRequest<TipoAsientoResponse>;
}
