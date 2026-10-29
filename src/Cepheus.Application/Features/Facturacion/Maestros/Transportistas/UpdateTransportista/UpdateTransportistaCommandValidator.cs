using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Transportistas.UpdateTransportista
{
    public class UpdateTransportistaCommandValidator : AbstractValidator<UpdateTransportistaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTransportistaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del transportista es obligatorio.")
                .Length(4).WithMessage("El código debe tener 4 caracteres.");

            RuleFor(x => x.DocumentTypeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de documento es obligatorio.")
                .MustAsync(DocumentTypeExists).WithMessage("El tipo de documento indicado no existe.");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número de documento es obligatorio.")
                .MaximumLength(20).WithMessage("El número de documento no puede exceder los 20 caracteres.");

            RuleFor(x => x)
                .MustAsync(BeUniqueDocument)
                .WithMessage("Ya existe otro transportista con ese tipo y número de documento.")
                .OverridePropertyName(nameof(UpdateTransportistaCommand.DocumentNumber));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");

            RuleFor(x => x.Address).MaximumLength(200).WithMessage("La dirección no puede exceder los 200 caracteres.");

            RuleFor(x => x.UbigeoCode)
                .MustAsync(UbigeoExists).WithMessage("El ubigeo indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.UbigeoCode));

            RuleFor(x => x.Phone).MaximumLength(30).WithMessage("El teléfono no puede exceder los 30 caracteres.");
            RuleFor(x => x.Email)
                .MaximumLength(150).WithMessage("El correo no puede exceder los 150 caracteres.")
                .EmailAddress().WithMessage("El correo no tiene un formato válido.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.MtcInternalCode).MaximumLength(20).WithMessage("El código interno del MTC no puede exceder los 20 caracteres.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> DocumentTypeExists(string code, CancellationToken ct)
            => await _uow.Comunes.TiposDocumento.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> BeUniqueDocument(UpdateTransportistaCommand command, CancellationToken ct)
            => !await _uow.Facturacion.Maestros.Transportistas.Query()
                .AnyAsync(t =>
                    t.Code != command.Code &&
                    t.DocumentTypeCode == command.DocumentTypeCode.Trim().ToUpper() &&
                    t.DocumentNumber == command.DocumentNumber.Trim(), ct);

        private async Task<bool> UbigeoExists(string? code, CancellationToken ct)
            => await _uow.Comunes.Ubigeos.Query().AnyAsync(u => u.Code == code!.Trim().ToUpper(), ct);
    }
}
