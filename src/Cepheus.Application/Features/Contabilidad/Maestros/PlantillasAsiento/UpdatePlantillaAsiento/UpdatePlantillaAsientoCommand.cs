using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.UpdatePlantillaAsiento
{
    /// <summary>Reemplaza el encabezado y el conjunto de líneas (se identifican por Order).</summary>
    public record UpdatePlantillaAsientoCommand(
        string Code,
        string Name,
        ProcesoAsiento Process,
        string TipoAsientoCode,
        string? PurchaseAccountCode,
        List<PlantillaAsientoLineaInput> Lines,
        byte[] RowVersion,
        string? ComprobantePagoCode = null,
        string? DescriptionText = null
    ) : IRequest<PlantillaAsientoResponse>, IPlantillaAsientoCabecera;
}
