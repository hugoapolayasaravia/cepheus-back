using Cepheus.Application.Features.Comunes.Monedas.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Monedas.CreateMoneda
{
    public record CreateMonedaCommand(
        string Code,
        string Name,
        string? Symbol,
        string? NumericCode,
        int DecimalPlaces
    ) : IRequest<MonedaResponse>;
}
