using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.UpdateMotivoChequeRebote
{
    public class UpdateMotivoChequeReboteCommandValidator : AbstractValidator<UpdateMotivoChequeReboteCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateMotivoChequeReboteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code).NotEmpty().Length(2).WithMessage("El código debe tener 2 caracteres.");
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("La versión de la fila es obligatoria.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del motivo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un motivo de rebote con ese nombre.");
        }

        private async Task<bool> BeUniqueName(UpdateMotivoChequeReboteCommand cmd, string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.MotivosChequeRebote.Query()
                .AnyAsync(t => t.Code != cmd.Code.Trim() && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
