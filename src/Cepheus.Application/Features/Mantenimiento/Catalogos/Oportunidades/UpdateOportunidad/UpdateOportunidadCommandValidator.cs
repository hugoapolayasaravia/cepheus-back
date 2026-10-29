using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.UpdateOportunidad
{
    public class UpdateOportunidadCommandValidator : AbstractValidator<UpdateOportunidadCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOportunidadCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la oportunidad es obligatorio.")
                .MaximumLength(2).WithMessage("El código no puede exceder los 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la oportunidad es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra oportunidad con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateOportunidadCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Catalogos.Oportunidades.Query()
                .AnyAsync(o => o.Code != command.Code && o.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
