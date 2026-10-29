using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.CreateFormaPago
{
    public class CreateFormaPagoCommandValidator : AbstractValidator<CreateFormaPagoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateFormaPagoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la forma de pago es obligatorio.")
                .MaximumLength(30).WithMessage("El nombre no puede exceder los 30 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una forma de pago con ese nombre.");

            RuleFor(x => x.Days)
                .GreaterThanOrEqualTo(0).WithMessage("El plazo en días no puede ser negativo.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.FormasPago.Query()
                .AnyAsync(f => f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}