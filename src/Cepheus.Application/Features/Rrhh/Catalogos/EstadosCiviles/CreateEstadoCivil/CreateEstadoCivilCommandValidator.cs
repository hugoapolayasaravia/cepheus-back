using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.CreateEstadoCivil
{
    public class CreateEstadoCivilCommandValidator : AbstractValidator<CreateEstadoCivilCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateEstadoCivilCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del estado civil es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un estado civil con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Catalogos.EstadosCiviles.Query()
                .AnyAsync(e => e.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}