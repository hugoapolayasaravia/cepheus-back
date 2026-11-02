using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.CreateFormaPagoCancelacion
{
    public class CreateFormaPagoCancelacionCommandValidator : AbstractValidator<CreateFormaPagoCancelacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateFormaPagoCancelacionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(1).WithMessage("El código debe tener 1 carácter.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe una forma de pago con ese código.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(50).WithMessage("La descripción no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una forma de pago con esa descripción.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
            => !await _uow.Contabilidad.Catalogos.FormasPagoCancelacion.Query()
                .AnyAsync(s => s.Code == code.Trim(), cancellationToken);

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Contabilidad.Catalogos.FormasPagoCancelacion.Query()
                .AnyAsync(s => s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
