using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.CreateCentroCosto
{
    public class CreateCentroCostoCommandValidator : AbstractValidator<CreateCentroCostoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCentroCostoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del centro de costo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un centro de costo con ese nombre.");

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Maestros.CentrosCosto.Query()
                .AnyAsync(c => c.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

        private async Task<bool> PlantaExists(string code, CancellationToken cancellationToken)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), cancellationToken);
    }
}
