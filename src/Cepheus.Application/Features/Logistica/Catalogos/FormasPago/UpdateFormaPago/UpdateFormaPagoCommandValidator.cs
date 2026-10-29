using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.UpdateFormaPago
{
    public class UpdateFormaPagoCommandValidator : AbstractValidator<UpdateFormaPagoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateFormaPagoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la forma de pago es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la forma de pago es obligatorio.")
                .MaximumLength(30).WithMessage("El nombre no puede exceder los 30 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra forma de pago con ese nombre.");

            RuleFor(x => x.Days)
                .GreaterThanOrEqualTo(0).WithMessage("El plazo en días no puede ser negativo.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateFormaPagoCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.FormasPago.Query()
                .AnyAsync(f => f.Code != command.Code && f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}