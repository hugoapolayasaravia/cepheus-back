using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.CreateSubCentroCosto
{
    public record CreateSubCentroCostoCommand(
        string? CentroCostoCode,
        string Name,
        string? AccountingAccountCode,
        string? AccountingAttachmentTypeCode,
        string PlantaCode,
        string? ParentCode
    ) : IRequest<SubCentroCostoResponse>;
}
