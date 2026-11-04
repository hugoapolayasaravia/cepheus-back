using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.RegistrarCobranzaDudosa
{
    public class RegistrarCobranzaDudosaCommandValidator : AbstractValidator<RegistrarCobranzaDudosaCommand>
    {
        public RegistrarCobranzaDudosaCommandValidator()
        {
            RuleFor(x => x.TypeCode).NotEmpty().Length(2);
            RuleFor(x => x.Number).NotEmpty().MaximumLength(15);
            RuleFor(x => x.PlantaCode).MaximumLength(2);
            RuleFor(x => x.ClienteCode).MaximumLength(5);
            RuleFor(x => x.BancoCode).MaximumLength(3);
            RuleFor(x => x.ProcessDate).NotEmpty().WithMessage("La fecha de proceso es obligatoria.");
            RuleFor(x => x.TipoContabilidad).IsInEnum();
        }
    }
}
