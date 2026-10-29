using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.CreateNivelEducativo
{
    public class CreateNivelEducativoCommandValidator : AbstractValidator<CreateNivelEducativoCommand>
    {
        private readonly IUnitOfWork _uow;
        public CreateNivelEducativoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del nivel educativo es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un nivel educativo con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        => !await _uow.Rrhh.Catalogos.NivelesEducativos.Query()
        .AnyAsync(r => r.Name == name.Trim(), cancellationToken);
    }
}