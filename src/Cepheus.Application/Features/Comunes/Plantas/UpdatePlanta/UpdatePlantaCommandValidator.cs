using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.UpdateFamilia;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Plantas.UpdatePlanta
{
    public class UpdatePlantaCommandValidator : AbstractValidator<UpdatePlantaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePlantaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código de la planta es obligatorio.")
                .MaximumLength(2)
                .MustAsync(BeUniqueCode).WithMessage("Ya existe una planta con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la planta es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra planta con ese nombre.");

            RuleFor(x => x.Address)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(150);

            RuleFor(x => x.UbigeoCode)
                .Length(6).When(x => !string.IsNullOrWhiteSpace(x.UbigeoCode))
                .WithMessage("El código de ubigeo debe tener 6 caracteres.");
        }

        private async Task<bool> BeUniqueCode(UpdatePlantaCommand command, string code, CancellationToken cancellationToken)
            => !await _uow.Comunes.Plantas.Query()
                .AnyAsync(p => p.Code == code.Trim().ToUpper() && p.Code != command.Code, cancellationToken);

        private async Task<bool> BeUniqueName(UpdatePlantaCommand command, string name, CancellationToken cancellationToken)
                => !await _uow.Comunes.Plantas.Query()
                    .AnyAsync(f => f.Code != command.Code && f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

    }
}
