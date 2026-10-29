using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.CreateProducto
{
    public class CreateProductoCommandHandler : IRequestHandler<CreateProductoCommand, ProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProductoResponse> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
        {
            var tipoProductoCode = request.TipoProductoCode.Trim().ToUpperInvariant();

            // Correlativo de 4 dígitos por tipo de producto (PK compuesta)
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Maestros.Productos.Query()
                    .Where(p => p.TipoProductoCode == tipoProductoCode)
                    .Select(p => p.Code),
                length: 4, entityLabel: $"Productos del tipo {tipoProductoCode}", cancellationToken);

            var producto = new Producto
            {
                TipoProductoCode = tipoProductoCode,
                Code = code,
                Name = request.Name.Trim(),
                ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim(),
                UnitCode = request.UnitCode.Trim().ToUpperInvariant(),
                AccountingAccountCode = string.IsNullOrWhiteSpace(request.AccountingAccountCode) ? null : request.AccountingAccountCode.Trim(),
                TransportAccountCode = string.IsNullOrWhiteSpace(request.TransportAccountCode) ? null : request.TransportAccountCode.Trim(),
                CreditNoteAccountCode = string.IsNullOrWhiteSpace(request.CreditNoteAccountCode) ? null : request.CreditNoteAccountCode.Trim(),
                LengthLimit = request.LengthLimit,
                TransportTipoProductoCode = string.IsNullOrWhiteSpace(request.TransportTipoProductoCode) ? null : request.TransportTipoProductoCode.Trim().ToUpperInvariant(),
                TransportCode = string.IsNullOrWhiteSpace(request.TransportCode) ? null : request.TransportCode.Trim().ToUpperInvariant(),
                CategoryCode = string.IsNullOrWhiteSpace(request.CategoryCode) ? null : request.CategoryCode.Trim().ToUpperInvariant(),
                StrengthCode = string.IsNullOrWhiteSpace(request.StrengthCode) ? null : request.StrengthCode.Trim(),
                CementTypeCode = string.IsNullOrWhiteSpace(request.CementTypeCode) ? null : request.CementTypeCode.Trim(),
                StoneSizeCode = string.IsNullOrWhiteSpace(request.StoneSizeCode) ? null : request.StoneSizeCode.Trim(),
                SlumpCode = string.IsNullOrWhiteSpace(request.SlumpCode) ? null : request.SlumpCode.Trim(),
                WaterCementRatioCode = string.IsNullOrWhiteSpace(request.WaterCementRatioCode) ? null : request.WaterCementRatioCode.Trim(),
                AgeCode = string.IsNullOrWhiteSpace(request.AgeCode) ? null : request.AgeCode.Trim(),
                SpecialConditionCode = string.IsNullOrWhiteSpace(request.SpecialConditionCode) ? null : request.SpecialConditionCode.Trim(),
                MixProportionCode = string.IsNullOrWhiteSpace(request.MixProportionCode) ? null : request.MixProportionCode.Trim(),
                IsPumpable = request.IsPumpable,
                IsSubjectToDetraction = request.IsSubjectToDetraction,
                GoodsTypeCode = string.IsNullOrWhiteSpace(request.GoodsTypeCode) ? null : request.GoodsTypeCode.Trim().ToUpperInvariant(),
                OperationTypeCode = string.IsNullOrWhiteSpace(request.OperationTypeCode) ? null : request.OperationTypeCode.Trim().ToUpperInvariant(),
                CementValue = request.CementValue,
                IsActive = true
            };

            await _uow.Facturacion.Maestros.Productos.AddAsync(producto, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(producto);
        }

        internal static ProductoResponse Map(Producto p) => new()
        {
            TipoProductoCode = p.TipoProductoCode,
            Code = p.Code,
            FullCode = p.FullCode,
            Name = p.Name,
            ShortName = p.ShortName,
            UnitCode = p.UnitCode,
            AccountingAccountCode = p.AccountingAccountCode,
            TransportAccountCode = p.TransportAccountCode,
            CreditNoteAccountCode = p.CreditNoteAccountCode,
            LengthLimit = p.LengthLimit,
            TransportTipoProductoCode = p.TransportTipoProductoCode,
            TransportCode = p.TransportCode,
            CategoryCode = p.CategoryCode,
            StrengthCode = p.StrengthCode,
            CementTypeCode = p.CementTypeCode,
            StoneSizeCode = p.StoneSizeCode,
            SlumpCode = p.SlumpCode,
            WaterCementRatioCode = p.WaterCementRatioCode,
            AgeCode = p.AgeCode,
            SpecialConditionCode = p.SpecialConditionCode,
            MixProportionCode = p.MixProportionCode,
            IsPumpable = p.IsPumpable,
            IsSubjectToDetraction = p.IsSubjectToDetraction,
            GoodsTypeCode = p.GoodsTypeCode,
            OperationTypeCode = p.OperationTypeCode,
            CementValue = p.CementValue,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            CreatedBy = p.CreatedBy,
            UpdatedBy = p.UpdatedBy,
            RowVersion = p.RowVersion
        };
    }
}
