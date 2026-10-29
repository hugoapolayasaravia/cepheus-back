using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.CreateGradoInstruccion
{
    public class CreateGradoInstruccionCommandValidator : AbstractValidator<CreateGradoInstruccionCommand>
    {
        private readonly IUnitOfWork _uow;
        public CreateGradoInstruccionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del grado de instrucción es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un grado académico con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        => !await _uow.Rrhh.Catalogos.GradosInstruccion.Query()
        .AnyAsync(r => r.Name == name.Trim(), cancellationToken);
    }
}