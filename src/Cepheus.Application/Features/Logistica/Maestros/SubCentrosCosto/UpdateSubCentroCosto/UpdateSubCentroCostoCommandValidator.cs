using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.UpdateSubCentroCosto
{
    public class UpdateSubCentroCostoCommandValidator : AbstractValidator<UpdateSubCentroCostoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSubCentroCostoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del subcentro de costo es obligatorio.")
                .Length(6).WithMessage("El código debe tener 6 caracteres.");

            RuleFor(x => x.CentroCostoCode)
                .MustAsync(CentroCostoExists).WithMessage("El centro de costo indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.CentroCostoCode));

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del subcentro de costo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro subcentro de costo con ese nombre.");

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.ParentCode)
                .MustAsync(ParentExists).WithMessage("El subcentro de costo padre indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ParentCode));

            RuleFor(x => x)
                .Must(x => !string.Equals(x.Code?.Trim(), x.ParentCode?.Trim(), StringComparison.OrdinalIgnoreCase))
                .WithMessage("Un subcentro de costo no puede ser padre de sí mismo.")
                .When(x => !string.IsNullOrWhiteSpace(x.ParentCode));

            RuleFor(x => x.AccountingAccountCode)
                .MustAsync(AccountingAccountExists).WithMessage("La cuenta contable indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.AccountingAccountCode));
            RuleFor(x => x.AccountingAttachmentTypeCode)
                .MustAsync(AccountingAttachmentTypeExists).WithMessage("El tipo de anexo contable indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.AccountingAttachmentTypeCode));


            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> CentroCostoExists(string? code, CancellationToken ct)
            => await _uow.Logistica.Maestros.CentrosCosto.Query().AnyAsync(c => c.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> BeUniqueName(UpdateSubCentroCostoCommand command, string name, CancellationToken ct)
            => !await _uow.Logistica.Maestros.SubCentrosCosto.Query()
                .AnyAsync(s => s.Code != command.Code && s.Name.ToLower() == name.Trim().ToLower(), ct);

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ParentExists(string? parentCode, CancellationToken ct)
            => await _uow.Logistica.Maestros.SubCentrosCosto.Query().AnyAsync(s => s.Code == parentCode!.Trim().ToUpper(), ct);

        private async Task<bool> AccountingAccountExists(string? code, CancellationToken ct)
            => await _uow.Contabilidad.Maestros.CuentasContables.Query().AnyAsync(c => c.Code == code!.Trim(), ct);

        private async Task<bool> AccountingAttachmentTypeExists(string? code, CancellationToken ct)
            => await _uow.Contabilidad.Catalogos.TiposAnexo.Query().AnyAsync(t => t.Code == code!.Trim(), ct);

    }
}
