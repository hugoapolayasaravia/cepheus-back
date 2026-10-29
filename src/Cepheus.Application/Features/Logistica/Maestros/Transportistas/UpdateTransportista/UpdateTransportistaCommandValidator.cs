using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.UpdateTransportista
{
    public class UpdateTransportistaCommandValidator : AbstractValidator<UpdateTransportistaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTransportistaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("El código del transportista es obligatorio.")
                .Length(5)
                .WithMessage("El código debe tener 5 caracteres.");

            RuleFor(x => x.DocumentTypeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El tipo de documento es obligatorio.")
                .MustAsync(DocumentTypeExists)
                .WithMessage("El tipo de documento indicado no existe.");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty()
                .WithMessage("El número de documento es obligatorio.")
                .MaximumLength(20)
                .WithMessage("El número de documento no puede exceder los 20 caracteres.");

            RuleFor(x => x)
                .MustAsync(BeUniqueDocument)
                .WithMessage("Ya existe otro transportista con ese tipo y número de documento.")
                .OverridePropertyName(nameof(UpdateTransportistaCommand.DocumentNumber));

            RuleFor(x => x.LegalName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("La razón social es obligatoria.")
                .MaximumLength(150)
                .WithMessage("La razón social no puede exceder los 150 caracteres.")
                .MustAsync(BeUniqueLegalName)
                .WithMessage("Ya existe otro transportista con esa razón social.");

            RuleFor(x => x.TradeName)
                .MaximumLength(150)
                .WithMessage("El nombre comercial no puede exceder los 150 caracteres.");

            RuleFor(x => x.Address)
                .MaximumLength(200)
                .WithMessage("La dirección no puede exceder los 200 caracteres.");

            RuleFor(x => x.UbigeoCode)
                .MustAsync(UbigeoExists)
                .WithMessage("El ubigeo indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.UbigeoCode));

            RuleFor(x => x.Phone)
                .MaximumLength(30)
                .WithMessage("El teléfono no puede exceder los 30 caracteres.");

            RuleFor(x => x.Email)
                .MaximumLength(150)
                .WithMessage("El correo no puede exceder los 150 caracteres.")
                .EmailAddress()
                .WithMessage("El correo no tiene un formato válido.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            // Registro MTC
            RuleFor(x => x.MtcRegistrationNumber)
                .MaximumLength(30)
                .WithMessage("El registro MTC no puede exceder los 30 caracteres.");

            RuleFor(x => x)
                .MustAsync(BeUniqueMtcRegistrationNumber)
                .WithMessage("Ya existe otro transportista con ese número de registro MTC.")
                .OverridePropertyName(nameof(UpdateTransportistaCommand.MtcRegistrationNumber))
                .When(x => !string.IsNullOrWhiteSpace(x.MtcRegistrationNumber));

            // Número de certificación
            RuleFor(x => x.CertificationNumber)
                .MaximumLength(40)
                .WithMessage("El número de certificación no puede exceder los 40 caracteres.");

            RuleFor(x => x)
                .MustAsync(BeUniqueCertificationNumber)
                .WithMessage("Ya existe otro transportista con ese número de certificación.")
                .OverridePropertyName(nameof(UpdateTransportistaCommand.CertificationNumber))
                .When(x => !string.IsNullOrWhiteSpace(x.CertificationNumber));

            RuleFor(x => x.Observations)
                .MaximumLength(500)
                .WithMessage("Las observaciones no pueden exceder los 500 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> DocumentTypeExists(
            string code,
            CancellationToken ct)
            => await _uow.Comunes.TiposDocumento.Query()
                .AnyAsync(
                    t => t.Code == code.Trim().ToUpper(),
                    ct);

        private async Task<bool> BeUniqueDocument(
            UpdateTransportistaCommand command,
            CancellationToken ct)
            => !await _uow.Logistica.Maestros.Transportistas.Query()
                .AnyAsync(
                    t =>
                        t.Code != command.Code &&
                        t.DocumentTypeCode == command.DocumentTypeCode.Trim().ToUpper() &&
                        t.DocumentNumber == command.DocumentNumber.Trim(),
                    ct);

        private async Task<bool> BeUniqueLegalName(
            UpdateTransportistaCommand command,
            string name,
            CancellationToken ct)
            => !await _uow.Logistica.Maestros.Transportistas.Query()
                .AnyAsync(
                    t =>
                        t.Code != command.Code &&
                        t.LegalName.ToLower() == name.Trim().ToLower(),
                    ct);

        private async Task<bool> BeUniqueMtcRegistrationNumber(
            UpdateTransportistaCommand command,
            CancellationToken ct)
            => !await _uow.Logistica.Maestros.Transportistas.Query()
                .AnyAsync(
                    t =>
                        t.Code != command.Code &&
                        t.MtcRegistrationNumber != null &&
                        t.MtcRegistrationNumber.Trim() ==
                        command.MtcRegistrationNumber!.Trim(),
                    ct);

        private async Task<bool> BeUniqueCertificationNumber(
            UpdateTransportistaCommand command,
            CancellationToken ct)
            => !await _uow.Logistica.Maestros.Transportistas.Query()
                .AnyAsync(
                    t =>
                        t.Code != command.Code &&
                        t.CertificationNumber != null &&
                        t.CertificationNumber.Trim() ==
                        command.CertificationNumber!.Trim(),
                    ct);

        private async Task<bool> UbigeoExists(
            string? code,
            CancellationToken ct)
            => await _uow.Comunes.Ubigeos.Query()
                .AnyAsync(
                    u => u.Code == code!.Trim().ToUpper(),
                    ct);
    }
}