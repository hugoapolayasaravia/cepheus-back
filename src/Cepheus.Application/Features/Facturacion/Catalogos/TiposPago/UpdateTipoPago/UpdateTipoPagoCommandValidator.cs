using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.UpdateTipoPago
{
    public class UpdateTipoPagoCommandValidator : AbstractValidator<UpdateTipoPagoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoPagoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code).NotEmpty().Length(2).WithMessage("El código debe tener 2 caracteres.");
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("La versión de la fila es obligatoria.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de pago con ese nombre.");

            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0).WithMessage("El orden no puede ser negativo.");

            RuleForEach(x => x.Accounts).ChildRules(a =>
            {
                a.RuleFor(i => i.MonedaCode).NotEmpty().MaximumLength(3).WithMessage("La moneda es obligatoria (ISO 4217).");
                a.RuleFor(i => i.AccountCode).NotEmpty().Length(8).WithMessage("La cuenta contable debe tener 8 caracteres.");
            });
        }

        private async Task<bool> BeUniqueName(UpdateTipoPagoCommand cmd, string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.TiposPago.Query()
                .AnyAsync(t => t.Code != cmd.Code.Trim() && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
