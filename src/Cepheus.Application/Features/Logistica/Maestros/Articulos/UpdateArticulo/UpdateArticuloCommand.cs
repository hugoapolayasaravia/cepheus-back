using Cepheus.Application.Features.Logistica.Maestros.Articulos.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.UpdateArticulo
{
    public record UpdateArticuloCommand(
        string Code,
        string Name,
        string UnidadMedidaCode,
        string SubFamiliaCode,
        decimal MinStock,
        decimal MaxStock,
        decimal IncomingStock,
        decimal LeadTimeDays,
        AbcClass AbcClass,
        string TipoArticuloCode,
        string PlanCode,
        string? ManufacturerCode,
        string? Observations,
        string? SalesTypeCode,
        string? SalesProductCode,
        bool IsAgreement,
        string? AccountingAccountCode,
        string? AccountingAttachmentTypeCode,
        string? PlantOriginCode,
        byte[] RowVersion
    ) : IRequest<ArticuloResponse>;
}
