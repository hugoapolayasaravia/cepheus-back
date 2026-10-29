using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.UpdateSubFamilia
{
    public class UpdateSubFamiliaCommandValidator : AbstractValidator<UpdateSubFamiliaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSubFamiliaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la subfamilia es obligatorio.")
                .Length(4).WithMessage("El código debe tener 4 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la subfamilia es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra subfamilia con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateSubFamiliaCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.SubFamilias.Query()
                .AnyAsync(s => s.Code != command.Code && s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}