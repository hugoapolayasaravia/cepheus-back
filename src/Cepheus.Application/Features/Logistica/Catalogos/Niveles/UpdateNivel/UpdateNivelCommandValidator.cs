using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.UpdateNivel
{
    public class UpdateNivelCommandValidator : AbstractValidator<UpdateNivelCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNivelCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code).NotEmpty();

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del nivel es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro nivel con ese nombre.");

            RuleFor(x => x.FechaInicio).NotEmpty().WithMessage("La fecha de inicio es obligatoria.");

            RuleFor(x => x.FechaFin)
                .GreaterThanOrEqualTo(x => x.FechaInicio)
                .WithMessage("La fecha de fin no puede ser anterior a la fecha de inicio.")
                .When(x => x.FechaFin.HasValue);

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateNivelCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.Niveles.Query()
                .AnyAsync(n => n.Code != command.Code && n.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
