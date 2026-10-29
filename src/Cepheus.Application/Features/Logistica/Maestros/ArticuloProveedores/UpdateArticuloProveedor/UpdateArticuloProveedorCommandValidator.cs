using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.UpdateArticuloProveedor
{
    public class UpdateArticuloProveedorCommandValidator : AbstractValidator<UpdateArticuloProveedorCommand>
    {
        public UpdateArticuloProveedorCommandValidator()
        {
            RuleFor(x => x.AgreementPrice)
                .GreaterThanOrEqualTo(0).WithMessage("El precio de convenio no puede ser negativo.")
                .When(x => x.AgreementPrice.HasValue);
        }
    }
}
