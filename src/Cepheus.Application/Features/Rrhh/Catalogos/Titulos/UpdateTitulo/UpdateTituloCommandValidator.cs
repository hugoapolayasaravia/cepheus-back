using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.UpdateNivelEducativo;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.UpdateTitulo
{
    public class UpdateTituloCommandValidator : AbstractValidator<UpdateTituloCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTituloCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del título es obligatorio.")
                .MaximumLength(20).WithMessage("El código no puede exceder los 20 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del título es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre no puede exceder los 200 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un título con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateTituloCommand command, string name, CancellationToken cancellationToken)
        => !await _uow.Rrhh.Catalogos.NivelesEducativos.Query()
        .AnyAsync(r => r.Name == name.Trim() && r.Code != command.Code, cancellationToken);
    }
}