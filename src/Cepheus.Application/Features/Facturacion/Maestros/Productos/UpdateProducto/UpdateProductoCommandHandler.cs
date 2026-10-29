using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.UpdateProducto
{
    public class UpdateProductoCommandHandler : IRequestHandler<UpdateProductoCommand, ProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProductoResponse> Handle(UpdateProductoCommand request, CancellationToken cancellationToken)
        {
            var tipoProductoCode = request.TipoProductoCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var current = await _uow.Facturacion.Maestros.Productos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TipoProductoCode == tipoProductoCode && p.Code == code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Producto {tipoProductoCode}{code} no encontrado.");
            }

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

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Maestros.Productos.Update(producto);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El producto fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateProducto.CreateProductoCommandHandler.Map(producto);
        }
    }
}
