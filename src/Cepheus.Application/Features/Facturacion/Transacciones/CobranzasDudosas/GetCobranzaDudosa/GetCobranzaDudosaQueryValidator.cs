using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GetCobranzaDudosa
{
    public class GetCobranzaDudosaQueryValidator : AbstractValidator<GetCobranzaDudosaQuery>
    {
        public GetCobranzaDudosaQueryValidator()
        {
            RuleFor(x => x.TypeCode).NotEmpty().Length(2);
            RuleFor(x => x.Number).NotEmpty().MaximumLength(15);
        }
    }
}
