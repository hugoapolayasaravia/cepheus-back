using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposCambio.GetTipoCambioByDate
{
    /// <summary>
    /// Caso de uso principal de consulta: obtener el tipo de cambio vigente de una moneda en una fecha puntual
    /// (ej. al emitir un comprobante en moneda extranjera).
    /// </summary>
    public record GetTipoCambioByDateQuery(string MonedaCode, DateOnly Date) : IRequest<TipoCambioResponse>;
}
