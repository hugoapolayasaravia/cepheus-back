using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.EmitirNotaDebitoVenta
{
    public class EmitirNotaDebitoVentaCommandValidator : AbstractValidator<EmitirNotaDebitoVentaCommand>
    {
        public EmitirNotaDebitoVentaCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().Length(2).WithMessage("El código de planta debe tener 2 caracteres.");
            RuleFor(x => x.Number).NotEmpty().MaximumLength(10).WithMessage("El número de la nota de débito es obligatorio.");
        }
    }
}
