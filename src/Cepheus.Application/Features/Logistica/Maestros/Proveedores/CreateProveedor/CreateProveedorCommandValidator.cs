using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.CreateProveedor
{
    public class CreateProveedorCommandValidator : AbstractValidator<CreateProveedorCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateProveedorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.DocumentTypeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de documento es obligatorio.")
                .Length(2).WithMessage("El código de tipo de documento debe tener 2 caracteres.")
                .MustAsync(DocumentTypeExists).WithMessage("El tipo de documento indicado no existe.");

            RuleFor(x => x.DocumentNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El número de documento (RUC/DNI) es obligatorio.")
                .MaximumLength(20).WithMessage("El número de documento no puede exceder los 20 caracteres.")
                .MustAsync(BeUniqueDocumentNumber).WithMessage("Ya existe un proveedor con ese número de documento.");

            RuleFor(x => x.LegalName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La razón social es obligatoria.")
                .MaximumLength(150).WithMessage("La razón social no puede exceder los 150 caracteres.")
                .MustAsync(BeUniqueLegalName).WithMessage("Ya existe un proveedor con esa razón social.");

            RuleFor(x => x.TradeName)
                .MaximumLength(150).WithMessage("El nombre comercial no puede exceder los 150 caracteres.");

            RuleFor(x => x.ProviderType)
                .IsInEnum().WithMessage("El tipo de proveedor no es válido.");

            RuleFor(x => x.Origin)
                .IsInEnum().WithMessage("La procedencia no es válida.");

            RuleFor(x => x.SunatCondition)
                .IsInEnum().WithMessage("La condición SUNAT no es válida.")
                .When(x => x.SunatCondition.HasValue);

            RuleFor(x => x.SunatStatus)
                .IsInEnum().WithMessage("El estado SUNAT no es válido.")
                .When(x => x.SunatStatus.HasValue);

            RuleFor(x => x.Observations)
                .MaximumLength(500).WithMessage("Las observaciones no pueden exceder los 500 caracteres.");
        }

        private async Task<bool> DocumentTypeExists(string documentTypeCode, CancellationToken cancellationToken)
            => await _uow.Comunes.TiposDocumento.Query()
                .AnyAsync(t => t.Code == documentTypeCode.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueDocumentNumber(string documentNumber, CancellationToken cancellationToken)
            => !await _uow.Logistica.Maestros.Proveedores.Query()
                .AnyAsync(p => p.DocumentNumber == documentNumber.Trim(), cancellationToken);

        private async Task<bool> BeUniqueLegalName(string legalName, CancellationToken cancellationToken)
            => !await _uow.Logistica.Maestros.Proveedores.Query()
                .AnyAsync(p => p.LegalName.ToLower() == legalName.Trim().ToLower(), cancellationToken);
    }
}
