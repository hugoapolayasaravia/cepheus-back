using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.UpdateCliente;
using Cepheus.Domain.Facturacion.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.UpdateProducto
{
    public class UpdateProductoCommandValidator : AbstractValidator<UpdateProductoCommand>
    {
        private const decimal MaxDecimal = 9999999999999999.99m;

        private readonly IUnitOfWork _uow;

        public UpdateProductoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TipoProductoCode)
                .NotEmpty().WithMessage("El tipo de producto es obligatorio.")
                .Length(2).WithMessage("El tipo de producto debe tener 2 caracteres.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del producto es obligatorio.")
                .Length(4).WithMessage("El código del producto debe tener 4 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(255).WithMessage("La descripción no puede exceder los 255 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro producto con ese nombre.");

            RuleFor(x => x.ShortName).MaximumLength(255).WithMessage("La descripción abreviada no puede exceder los 255 caracteres.");

            RuleFor(x => x.UnitCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La unidad de medida es obligatoria.")
                .MaximumLength(5).WithMessage("La unidad no puede exceder los 5 caracteres.")
                .MustAsync(UnitExists).WithMessage("La unidad de medida indicada no existe.");

            RuleFor(x => x.LengthLimit)
                .InclusiveBetween(0m, MaxDecimal).WithMessage("La longitud tope debe ser un valor positivo.");

            RuleFor(x => x)
                .Must(x => string.IsNullOrWhiteSpace(x.TransportTipoProductoCode) == string.IsNullOrWhiteSpace(x.TransportCode))
                .WithMessage("El producto de transporte requiere tipo y código, o ninguno de los dos.")
                .OverridePropertyName(nameof(UpdateProductoCommand.TransportCode));

            RuleFor(x => x)
                .MustAsync(TransportProductExists)
                .WithMessage("El producto de transporte indicado no existe.")
                .OverridePropertyName(nameof(UpdateProductoCommand.TransportCode))
                .When(x => !string.IsNullOrWhiteSpace(x.TransportTipoProductoCode) && !string.IsNullOrWhiteSpace(x.TransportCode));

            RuleFor(x => x)
                .Must(x => !(string.Equals(x.TransportTipoProductoCode?.Trim(), x.TipoProductoCode.Trim(), StringComparison.OrdinalIgnoreCase) &&
                             string.Equals(x.TransportCode?.Trim(), x.Code.Trim(), StringComparison.OrdinalIgnoreCase)))
                .WithMessage("El producto de transporte no puede ser el mismo producto.")
                .OverridePropertyName(nameof(UpdateProductoCommand.TransportCode))
                .When(x => !string.IsNullOrWhiteSpace(x.TransportTipoProductoCode) && !string.IsNullOrWhiteSpace(x.TransportCode));

            RuleFor(x => x.CategoryCode)
                .MustAsync(CategoryExists).WithMessage("La categoría indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.CategoryCode));

            AddAttributeRule(x => x.StrengthCode, TipoAtributoConcreto.Resistencia, "La resistencia");
            AddAttributeRule(x => x.CementTypeCode, TipoAtributoConcreto.TipoCemento, "El tipo de cemento");
            AddAttributeRule(x => x.StoneSizeCode, TipoAtributoConcreto.TamanoPiedra, "El tamaño de piedra");
            AddAttributeRule(x => x.SlumpCode, TipoAtributoConcreto.Slump, "El slump");
            AddAttributeRule(x => x.WaterCementRatioCode, TipoAtributoConcreto.RelacionAguaCemento, "La relación agua/cemento");
            AddAttributeRule(x => x.AgeCode, TipoAtributoConcreto.Edad, "La edad");
            AddAttributeRule(x => x.SpecialConditionCode, TipoAtributoConcreto.CondicionEspecial, "La condición especial");
            AddAttributeRule(x => x.MixProportionCode, TipoAtributoConcreto.ProporcionMezcla, "La proporción de mezcla");

            RuleFor(x => x.GoodsTypeCode)
                .NotEmpty().WithMessage("Un producto afecto a detracción requiere el tipo de bien.")
                .When(x => x.IsSubjectToDetraction);

            RuleFor(x => x.GoodsTypeCode)
                .MustAsync(GoodsTypeExists).WithMessage("El tipo de bien indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.GoodsTypeCode));

            RuleFor(x => x.OperationTypeCode)
                .MustAsync(OperationTypeExists).WithMessage("El tipo de operación indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.OperationTypeCode));

            RuleFor(x => x.CementValue)
                .Must(v => v!.Value >= 0m && v.Value <= MaxDecimal)
                .WithMessage("El valor del cemento debe ser un valor positivo.")
                .When(x => x.CementValue.HasValue);

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");

            RuleFor(x => x.AccountingAccountCode).MaximumLength(8).WithMessage("La cuenta contable no puede exceder los 8 caracteres.");
            RuleFor(x => x.TransportAccountCode).MaximumLength(8).WithMessage("La cuenta de transporte no puede exceder los 8 caracteres.");
            RuleFor(x => x.CreditNoteAccountCode).MaximumLength(8).WithMessage("La cuenta de nota de crédito no puede exceder los 8 caracteres.");

            RuleFor(x => x.AccountingAccountCode)
                .MustAsync(AccountingAccountExists).WithMessage("La cuenta contable indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.AccountingAccountCode));
            RuleFor(x => x.TransportAccountCode)
                .MustAsync(AccountingAccountExists).WithMessage("La cuenta de transporte indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.TransportAccountCode));
            RuleFor(x => x.CreditNoteAccountCode)
                .MustAsync(AccountingAccountExists).WithMessage("La cuenta de nota de crédito indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.CreditNoteAccountCode));
        }

        private void AddAttributeRule(Expression<Func<UpdateProductoCommand, string?>> selector, TipoAtributoConcreto type, string label)
        {
            var getter = selector.Compile();

            RuleFor(selector)
                .MustAsync((code, ct) => AttributeIsOfType(code, type, ct))
                .WithMessage($"{label} indicado no existe o no corresponde a ese tipo de atributo.")
                .When(x => !string.IsNullOrWhiteSpace(getter(x)));
        }

        private async Task<bool> TipoProductoExists(string code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.TiposProducto.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> BeUniqueName(UpdateProductoCommand command, string? name, CancellationToken ct)
        => !await _uow.Facturacion.Maestros.Productos.Query()
            .AnyAsync(c => c.Code != command.Code && c.Name != null && c.Name.ToLower() == name!.Trim().ToLower(), ct);

        private async Task<bool> UnitExists(string code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.UnidadesMedidaVenta.Query().AnyAsync(u => u.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> CategoryExists(string? code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.CategoriasProducto.Query().AnyAsync(c => c.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> GoodsTypeExists(string? code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.TiposBien.Query().AnyAsync(g => g.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> OperationTypeExists(string? code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.TiposOperacion.Query().AnyAsync(o => o.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> AttributeIsOfType(string? code, TipoAtributoConcreto type, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.AtributosConcreto.Query()
                .AnyAsync(a => a.Code == code!.Trim() && a.AttributeType == type, ct);

        private async Task<bool> TransportProductExists(UpdateProductoCommand command, CancellationToken ct)
        {
            var tipo = command.TransportTipoProductoCode!.Trim().ToUpper();
            var code = command.TransportCode!.Trim().ToUpper();

            return await _uow.Facturacion.Maestros.Productos.Query()
                .AnyAsync(p => p.TipoProductoCode == tipo && p.Code == code, ct);
        }

        private async Task<bool> AccountingAccountExists(string? code, CancellationToken ct)
            => await _uow.Contabilidad.Maestros.CuentasContables.Query().AnyAsync(c => c.Code == code!.Trim(), ct);

    }
}
