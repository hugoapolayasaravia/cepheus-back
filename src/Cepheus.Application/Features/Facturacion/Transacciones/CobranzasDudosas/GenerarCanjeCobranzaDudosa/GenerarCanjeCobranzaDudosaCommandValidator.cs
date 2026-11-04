using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GenerarCanjeCobranzaDudosa
{
    public class GenerarCanjeCobranzaDudosaCommandValidator : AbstractValidator<GenerarCanjeCobranzaDudosaCommand>
    {
        public GenerarCanjeCobranzaDudosaCommandValidator()
        {
            RuleFor(x => x.TypeCode).NotEmpty().Length(2);
            RuleFor(x => x.Number).NotEmpty().MaximumLength(15);
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
