using Cepheus.Application.Features.Facturacion.Maestros.Productos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.CreateProducto
{
    public record CreateProductoCommand(
        string TipoProductoCode,
        string Name,
        string? ShortName,
        string UnitCode,
        string? AccountingAccountCode,
        string? TransportAccountCode,
        string? CreditNoteAccountCode,
        decimal LengthLimit,
        string? TransportTipoProductoCode,
        string? TransportCode,
        string? CategoryCode,
        string? StrengthCode,
        string? CementTypeCode,
        string? StoneSizeCode,
        string? SlumpCode,
        string? WaterCementRatioCode,
        string? AgeCode,
        string? SpecialConditionCode,
        string? MixProportionCode,
        bool IsPumpable,
        bool IsSubjectToDetraction,
        string? GoodsTypeCode,
        string? OperationTypeCode,
        decimal? CementValue
    ) : IRequest<ProductoResponse>;
}
