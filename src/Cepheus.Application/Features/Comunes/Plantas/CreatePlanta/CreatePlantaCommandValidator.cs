using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Plantas.CreatePlanta
{
    public class CreatePlantaCommandValidator : AbstractValidator<CreatePlantaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreatePlantaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código de la planta es obligatorio.")
                .MaximumLength(2).WithMessage("El código no puede exceder los 10 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe una planta con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la planta es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una familia con ese nombre.");

            RuleFor(x => x.LegalName)
                .MaximumLength(150).WithMessage("La razón social no puede exceder los 150 caracteres.");

            RuleFor(x => x.Address)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(150).WithMessage("La dirección no puede exceder los 150 caracteres.");

            RuleFor(x => x.AddressComplement)
                .MaximumLength(150).WithMessage("El complemento de dirección no puede exceder los 150 caracteres.");

            RuleFor(x => x.UbigeoCode)
                .Length(6).When(x => !string.IsNullOrWhiteSpace(x.UbigeoCode))
                .WithMessage("El código de ubigeo debe tener 6 caracteres.");

            RuleFor(x => x.ManagerName)
                .MaximumLength(100).WithMessage("El responsable no puede exceder los 100 caracteres.");

            RuleFor(x => x.StatusCode)
                .MaximumLength(2).WithMessage("El código de estado no puede exceder los 2 caracteres.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
            => !await _uow.Comunes.Plantas.Query()
                .AnyAsync(p => p.Code == code.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Comunes.Plantas.Query()
                .AnyAsync(f => f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
