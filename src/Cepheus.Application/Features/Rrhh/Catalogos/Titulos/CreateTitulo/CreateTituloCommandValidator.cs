using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.CreateTitulo
{
    public class CreateTituloCommandValidator : AbstractValidator<CreateTituloCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTituloCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del título es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre no puede exceder los 200 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un título con ese nombre.");
        }
        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Catalogos.Titulos.Query()
                .AnyAsync(r => r.Name == name.Trim(), cancellationToken);
    }
}