using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.CreateTipoAnexo
{
    public class CreateTipoAnexoCommandValidator : AbstractValidator<CreateTipoAnexoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoAnexoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de anexo con esa descripción.");

            RuleFor(x => x.Flag)
                .MaximumLength(1).WithMessage("El flag no puede exceder 1 caracter.");

        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Contabilidad.Catalogos.TiposAnexo.Query()
                .AnyAsync(s => s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
