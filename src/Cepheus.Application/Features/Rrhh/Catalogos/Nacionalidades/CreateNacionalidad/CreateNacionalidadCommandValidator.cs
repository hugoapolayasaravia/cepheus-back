using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.CreateNacionalidad
{
    public class CreateNacionalidadCommandValidator : AbstractValidator<CreateNacionalidadCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateNacionalidadCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la nacionalidad es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una nacionalidad con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Catalogos.Nacionalidades.Query()
                .AnyAsync(n => n.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}