using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Negocios.UpdateNegocio
{
    public class UpdateNegocioCommandValidator : AbstractValidator<UpdateNegocioCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNegocioCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del negocio es obligatorio.")
                .MaximumLength(2).WithMessage("El código no puede exceder los 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del negocio es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro negocio con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateNegocioCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Comunes.Negocios.Query()
                .AnyAsync(n => n.Code != command.Code && n.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
