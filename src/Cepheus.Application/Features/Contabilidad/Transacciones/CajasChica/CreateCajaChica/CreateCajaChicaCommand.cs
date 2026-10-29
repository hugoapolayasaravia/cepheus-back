using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.CreateCajaChica
{
    public record CreateCajaChicaCommand(
        string PlantaCode,
        string TrabajadorCode,
        DateTime IssueDate,
        string? Observations,
        string Type,
        List<CajaChicaDetalleInput>? Details
    ) : IRequest<CajaChicaResponse>;
}
