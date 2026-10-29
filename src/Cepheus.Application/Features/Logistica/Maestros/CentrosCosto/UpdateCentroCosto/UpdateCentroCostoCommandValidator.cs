using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.UpdateCentroCosto
{
    public class UpdateCentroCostoCommandValidator : AbstractValidator<UpdateCentroCostoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCentroCostoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del centro de costo es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del centro de costo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro centro de costo con ese nombre.");

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateCentroCostoCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Maestros.CentrosCosto.Query()
                .AnyAsync(c => c.Code != command.Code && c.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

        private async Task<bool> PlantaExists(string code, CancellationToken cancellationToken)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), cancellationToken);
    }
}
