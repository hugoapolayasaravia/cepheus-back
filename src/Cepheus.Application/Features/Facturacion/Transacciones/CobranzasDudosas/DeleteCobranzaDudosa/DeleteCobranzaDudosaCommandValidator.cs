using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.DeleteCobranzaDudosa
{
    public class DeleteCobranzaDudosaCommandValidator : AbstractValidator<DeleteCobranzaDudosaCommand>
    {
        public DeleteCobranzaDudosaCommandValidator()
        {
            RuleFor(x => x.TypeCode).NotEmpty().Length(2);
            RuleFor(x => x.Number).NotEmpty().MaximumLength(15);
        }
    }
}
