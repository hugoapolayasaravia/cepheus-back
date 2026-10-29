using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.UpdateEstadoCivil
{
    public class UpdateEstadoCivilCommandValidator : AbstractValidator<UpdateEstadoCivilCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEstadoCivilCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del estado civil es obligatorio.")
                .MaximumLength(10).WithMessage("El código no puede exceder los 10 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del estado civil es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro estado civil con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateEstadoCivilCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Catalogos.EstadosCiviles.Query()
                .AnyAsync(e => e.Code != command.Code && e.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}