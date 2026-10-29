using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.CreateArticuloProveedor
{
    public class CreateArticuloProveedorCommandValidator : AbstractValidator<CreateArticuloProveedorCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateArticuloProveedorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.ArticuloCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El artículo es obligatorio.")
                .MustAsync(ArticuloExists).WithMessage("El artículo indicado no existe.");

            RuleFor(x => x.ProveedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El proveedor es obligatorio.")
                .MustAsync(ProveedorExists).WithMessage("El proveedor indicado no existe.");

            RuleFor(x => x)
                .MustAsync(BeUniqueCombination)
                .WithMessage("Ya existe esta combinación de planta, artículo y proveedor.")
                .OverridePropertyName(nameof(CreateArticuloProveedorCommand.ProveedorCode));

            RuleFor(x => x.AgreementPrice)
                .GreaterThanOrEqualTo(0).WithMessage("El precio de convenio no puede ser negativo.")
                .When(x => x.AgreementPrice.HasValue);
        }

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ArticuloExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Articulos.Query().AnyAsync(a => a.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ProveedorExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Proveedores.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> BeUniqueCombination(CreateArticuloProveedorCommand command, CancellationToken ct)
            => !await _uow.Logistica.Maestros.ArticuloProveedores.Query()
                .AnyAsync(x =>
                    x.PlantaCode == command.PlantaCode.Trim().ToUpper() &&
                    x.ArticuloCode == command.ArticuloCode.Trim().ToUpper() &&
                    x.ProveedorCode == command.ProveedorCode.Trim().ToUpper(), ct);
    }
}
