using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.ProvisionarCajaChica
{
    /// <summary>
    /// Pase a contabilidad: genera el asiento de provisión (plantilla ProvisionCajaChica) y los documentos
    /// de compra de la caja, y deja la caja en Procesado.
    /// </summary>
    public record ProvisionarCajaChicaCommand(string Code) : IRequest<CajaChicaResponse>;
}
