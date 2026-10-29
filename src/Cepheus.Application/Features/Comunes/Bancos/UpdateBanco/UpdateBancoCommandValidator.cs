using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Bancos.UpdateBanco
{
    public class UpdateBancoCommandValidator : AbstractValidator<UpdateBancoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateBancoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del banco es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del banco es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro banco con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateBancoCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Comunes.Bancos.Query()
                .AnyAsync(b => b.Code != command.Code && b.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
