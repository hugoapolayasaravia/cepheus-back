using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.UpdateSexo
{
    public class UpdateSexoCommandValidator : AbstractValidator<UpdateSexoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSexoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del sexo es obligatorio.")
                .MaximumLength(10).WithMessage("El código no puede exceder los 10 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del sexo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro sexo con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateSexoCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Catalogos.Sexos.Query()
                .AnyAsync(s => s.Code != command.Code && s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}