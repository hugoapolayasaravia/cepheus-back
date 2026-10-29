using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.UpdateTipoTransaccion
{
    public class UpdateTipoTransaccionCommandValidator : AbstractValidator<UpdateTipoTransaccionCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoTransaccionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del tipo de transacción es obligatorio.")
                .MaximumLength(2).WithMessage("El código no puede exceder los 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de transacción es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro tipo de transacción con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateTipoTransaccionCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.TiposTransaccion.Query()
                .AnyAsync(t => t.Code != command.Code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
