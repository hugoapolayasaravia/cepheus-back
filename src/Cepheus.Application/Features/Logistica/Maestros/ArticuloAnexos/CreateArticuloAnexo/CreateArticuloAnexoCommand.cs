using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.CreateArticuloAnexo
{
    public record CreateArticuloAnexoCommand(
        string SubCentroCostoCode,
        string ArticuloCode,
        string AccountingAccountCode,
        string? AccountingAttachmentTypeCode,
        string? AccountingAttachmentCode
    ) : IRequest<ArticuloAnexoResponse>, IArticuloAnexoDatos;
}
