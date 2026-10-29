using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.UpdateSubFamiliaAnexo;

/// <summary>La clave (subcentro + subfamilia) no se modifica; solo la cuenta, el tipo de anexo y el anexo.</summary>
public record UpdateSubFamiliaAnexoCommand(
    string SubCentroCostoCode,
    string SubFamiliaCode,
    string AccountingAccountCode,
    string? AccountingAttachmentTypeCode,
    string? AccountingAttachmentCode,
    byte[] RowVersion
) : IRequest<SubFamiliaAnexoResponse>;
