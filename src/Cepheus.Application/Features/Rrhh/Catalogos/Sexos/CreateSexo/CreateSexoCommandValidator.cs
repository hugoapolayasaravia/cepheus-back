using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.CreateSexo
{
    public class CreateSexoCommandValidator : AbstractValidator<CreateSexoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateSexoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del sexo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un sexo con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Catalogos.Sexos.Query()
                .AnyAsync(s => s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}