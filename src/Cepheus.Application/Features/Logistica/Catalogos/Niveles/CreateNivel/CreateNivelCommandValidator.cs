using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.CreateNivel
{
    public class CreateNivelCommandValidator : AbstractValidator<CreateNivelCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateNivelCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del nivel es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un nivel con ese nombre.");

            RuleFor(x => x.FechaFin)
                .GreaterThanOrEqualTo(x => x.FechaInicio ?? DateTime.UtcNow.Date)
                .WithMessage("La fecha de fin no puede ser anterior a la fecha de inicio.")
                .When(x => x.FechaFin.HasValue);
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.Niveles.Query()
                .AnyAsync(n => n.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
