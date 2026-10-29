using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.UpdateFamilia
{
    public class UpdateFamiliaCommandValidator : AbstractValidator<UpdateFamiliaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateFamiliaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la familia es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la familia es obligatorio.")
                .MaximumLength(60).WithMessage("El nombre no puede exceder los 60 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra familia con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateFamiliaCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.Familias.Query()
                .AnyAsync(f => f.Code != command.Code && f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}