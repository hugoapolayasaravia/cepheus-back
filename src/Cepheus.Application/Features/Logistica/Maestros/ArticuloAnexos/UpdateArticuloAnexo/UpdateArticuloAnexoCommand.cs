using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.UpdateArticuloAnexo
{
    /// <summary>La clave (subcentro + artículo) no cambia; se actualizan la cuenta y el anexo.</summary>
    public record UpdateArticuloAnexoCommand(
        string SubCentroCostoCode,
        string ArticuloCode,
        string AccountingAccountCode,
        string? AccountingAttachmentTypeCode,
        string? AccountingAttachmentCode,
        byte[] RowVersion
    ) : IRequest<ArticuloAnexoResponse>, IArticuloAnexoDatos;
}
