using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.CreatePeriodoContable
{
    public class CreatePeriodoContableCommandValidator : AbstractValidator<CreatePeriodoContableCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreatePeriodoContableCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Year)
                .InclusiveBetween(2000, 2100).WithMessage("El año debe estar entre 2000 y 2100.");

            RuleFor(x => x.Month)
                .InclusiveBetween(1, 12).WithMessage("El mes debe estar entre 1 y 12.");

            RuleFor(x => x)
                .MustAsync(BeUnique).WithMessage("El período contable ya está registrado.")
                .OverridePropertyName("Period")
                .When(x => x.Year is >= 2000 and <= 2100 && x.Month is >= 1 and <= 12);
        }

        private async Task<bool> BeUnique(CreatePeriodoContableCommand command, CancellationToken cancellationToken)
        {
            var code = $"{command.Year:0000}{command.Month:00}";
            return !await _uow.Contabilidad.Maestros.PeriodosContables.Query()
                .AnyAsync(p => p.Code == code, cancellationToken);
        }
    }
}
