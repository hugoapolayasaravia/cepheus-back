using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.UpdateConductor
{
    public class UpdateConductorCommandValidator : AbstractValidator<UpdateConductorCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateConductorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del conductor es obligatorio.")
                .Length(5).WithMessage("El código debe tener 5 caracteres.");

            RuleFor(x => x.DocumentTypeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de documento es obligatorio.")
                .MustAsync(DocumentTypeExists).WithMessage("El tipo de documento indicado no existe.");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número de documento es obligatorio.")
                .MaximumLength(20).WithMessage("El número de documento no puede exceder los 20 caracteres.");

            RuleFor(x => x)
                .MustAsync(BeUniqueDocument)
                .WithMessage("Ya existe otro conductor con ese tipo y número de documento.")
                .OverridePropertyName(nameof(UpdateConductorCommand.DocumentNumber));

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre del conductor es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido del conductor es obligatorio.")
                .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.");

            RuleFor(x => x.DriverLicenseNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El número de licencia de conducir es obligatorio.")
                .MaximumLength(20).WithMessage("El número de licencia no puede exceder los 20 caracteres.")
                .MustAsync(BeUniqueLicense).WithMessage("Ya existe otro conductor con ese número de licencia.");

            RuleFor(x => x.LicenseCategory).MaximumLength(10);
            RuleFor(x => x.Phone).MaximumLength(30);
            RuleFor(x => x.Email)
                .MaximumLength(150)
                .EmailAddress().WithMessage("El correo no tiene un formato válido.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.Observations).MaximumLength(500);

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> DocumentTypeExists(string code, CancellationToken ct)
            => await _uow.Comunes.TiposDocumento.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> BeUniqueDocument(UpdateConductorCommand command, CancellationToken ct)
            => !await _uow.Logistica.Maestros.Conductores.Query()
                .AnyAsync(c =>
                    c.Code != command.Code &&
                    c.DocumentTypeCode == command.DocumentTypeCode.Trim().ToUpper() &&
                    c.DocumentNumber == command.DocumentNumber.Trim(), ct);

        private async Task<bool> BeUniqueLicense(UpdateConductorCommand command, string license, CancellationToken ct)
            => !await _uow.Logistica.Maestros.Conductores.Query()
                .AnyAsync(c => c.Code != command.Code && c.DriverLicenseNumber == license.Trim().ToUpper(), ct);
    }
}
