using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.UpdateNacionalidad
{
    public class UpdateNacionalidadCommandValidator : AbstractValidator<UpdateNacionalidadCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNacionalidadCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la nacionalidad es obligatorio.")
                .MaximumLength(10).WithMessage("El código no puede exceder los 10 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la nacionalidad es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra nacionalidad con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateNacionalidadCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Catalogos.Nacionalidades.Query()
                .AnyAsync(n => n.Code != command.Code && n.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}