using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.UpdateMotivoDevolucion
{
    public class UpdateMotivoDevolucionCommandValidator : AbstractValidator<UpdateMotivoDevolucionCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateMotivoDevolucionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El código del motivo de devolución es obligatorio.")
            .MaximumLength(2).WithMessage("El código no puede exceder los 2 caracteres.");

            RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El nombre del motivo de devolución es obligatorio.")
            .MaximumLength(80).WithMessage("El nombre no puede exceder los 80 caracteres.")
            .MustAsync(BeUniqueName).WithMessage("Ya existe un motivo de devolución con ese nombre.");

        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Comunes.MotivosDevolucion.Query()
                .AnyAsync(b => b.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
