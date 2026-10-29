using Cepheus.Application.Features.Comunes.Monedas.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Monedas.UpdateMoneda
{
    public record UpdateMonedaCommand(
        string Code,
        string Name,
        string? Symbol,
        string? NumericCode,
        int DecimalPlaces,
        byte[] RowVersion
    ) : IRequest<MonedaResponse>;
}
