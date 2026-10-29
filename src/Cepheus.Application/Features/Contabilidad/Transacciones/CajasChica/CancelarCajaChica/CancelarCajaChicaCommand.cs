using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.CancelarCajaChica
{
    /// <summary>
    /// Cancela una caja Procesada: marca como pagados sus documentos de compra y la deja Cancelada.
    /// Fondo fijo (F) y por rendir (R) además generan el asiento de reversión (plantilla ReversionCajaChica)
    /// en el año y mes indicados; en una caja de gastos (G) los campos de reversión no aplican.
    /// </summary>
    public record CancelarCajaChicaCommand(
        string Code,
        string? ReversionYear,
        string? ReversionMonth
    ) : IRequest<CajaChicaResponse>;
}
