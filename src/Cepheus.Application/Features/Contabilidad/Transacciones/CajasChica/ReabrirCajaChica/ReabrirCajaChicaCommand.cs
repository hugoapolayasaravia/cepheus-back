using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.ReabrirCajaChica
{
    /// <summary>
    /// Devuelve una caja Procesada o Cancelada a Pendiente: anula el asiento de reversión (si existe) y el de
    /// provisión, libera notas de ingreso y órdenes de servicio, y elimina los documentos de compra que
    /// generó la caja. Resume en un solo paso los pasos sucesivos de ue_set_btn_eli_bor del legacy.
    /// </summary>
    public record ReabrirCajaChicaCommand(string Code) : IRequest<CajaChicaResponse>;
}
