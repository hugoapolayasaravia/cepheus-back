using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.CreateSubFamiliaAnexo;

/// <summary>El centro de costo no se envía: sale del SubCentro de Costo (Codigo_Cos del legacy).</summary>
public record CreateSubFamiliaAnexoCommand(
    string SubCentroCostoCode,
    string SubFamiliaCode,
    string AccountingAccountCode,
    string? AccountingAttachmentTypeCode,
    string? AccountingAttachmentCode
) : IRequest<SubFamiliaAnexoResponse>;
