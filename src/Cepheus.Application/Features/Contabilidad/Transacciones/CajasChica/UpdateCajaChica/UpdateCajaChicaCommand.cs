using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.UpdateCajaChica
{
    /// <summary>
    /// Modifica la cabecera y reemplaza todas las líneas. Solo una caja Pendiente y sin asientos.
    /// </summary>
    public record UpdateCajaChicaCommand(
        string Code,
        string PlantaCode,
        string TrabajadorCode,
        DateTime IssueDate,
        string? Observations,
        string Type,
        List<CajaChicaDetalleInput>? Details,
        byte[] RowVersion
    ) : IRequest<CajaChicaResponse>;
}
