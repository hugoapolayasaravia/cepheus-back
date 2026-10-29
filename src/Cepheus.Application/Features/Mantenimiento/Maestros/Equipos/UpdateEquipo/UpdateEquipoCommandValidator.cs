using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.UpdateEquipo
{
    public class UpdateEquipoCommandValidator : AbstractValidator<UpdateEquipoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEquipoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del equipo es obligatorio.")
                .MaximumLength(8).WithMessage("El código no puede exceder los 8 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del equipo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro equipo con ese nombre.");

            RuleFor(x => x.Nivel)
                .GreaterThanOrEqualTo(0).WithMessage("El nivel no puede ser negativo.");

            RuleFor(x => x.SubCentroCostoCode)
                .MustAsync(SubCentroCostoExists).WithMessage("El subcentro de costo indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.SubCentroCostoCode));

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateEquipoCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Maestros.Equipos.Query()
                .AnyAsync(e => e.Code != command.Code && e.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

        private async Task<bool> SubCentroCostoExists(string? code, CancellationToken cancellationToken)
            => await _uow.Logistica.Maestros.SubCentrosCosto.Query()
                .AnyAsync(s => s.Code == code!.Trim().ToUpper(), cancellationToken);
    }
}
