using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.UpdateGradoInstruccion;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.UpdateNivelEducativo
{
    public class UpdateNivelEducativoCommandValidator : AbstractValidator<UpdateNivelEducativoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNivelEducativoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del nivel educativo es obligatorio.")
                .MaximumLength(20).WithMessage("El código no puede exceder los 20 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del nivel educativo es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un nivel educativo con ese nombre."); 
            
            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateNivelEducativoCommand command, string name, CancellationToken cancellationToken)
        => !await _uow.Rrhh.Catalogos.NivelesEducativos.Query()
        .AnyAsync(r => r.Name == name.Trim() && r.Code != command.Code, cancellationToken);
    }
}