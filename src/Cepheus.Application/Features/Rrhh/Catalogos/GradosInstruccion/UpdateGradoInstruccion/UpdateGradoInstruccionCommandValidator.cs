using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.UpdateGradoInstruccion
{
    public class UpdateGradoInstruccionCommandValidator : AbstractValidator<UpdateGradoInstruccionCommand>
    {
        private readonly IUnitOfWork _uow;
        public UpdateGradoInstruccionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del grado de instrucción es obligatorio.")
                .MaximumLength(20).WithMessage("El código no puede exceder los 20 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del grado de instrucción es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un grado de instrucción  con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateGradoInstruccionCommand command, string name, CancellationToken cancellationToken)
        => !await _uow.Rrhh.Catalogos.GradosInstruccion.Query()
        .AnyAsync(r => r.Name == name.Trim() && r.Code != command.Code, cancellationToken);
    }
}