using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.UpdateSubCentroCosto
{
    public record UpdateSubCentroCostoCommand(
        string Code,
        string? CentroCostoCode,
        string Name,
        string? AccountingAccountCode,
        string? AccountingAttachmentTypeCode,
        string PlantaCode,
        string? ParentCode,
        byte[] RowVersion
    ) : IRequest<SubCentroCostoResponse>;
}
