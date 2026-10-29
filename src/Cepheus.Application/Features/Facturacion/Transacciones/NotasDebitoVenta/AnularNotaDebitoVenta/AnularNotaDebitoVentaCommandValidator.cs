using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.AnularNotaDebitoVenta
{
    public class AnularNotaDebitoVentaCommandValidator : AbstractValidator<AnularNotaDebitoVentaCommand>
    {
        public AnularNotaDebitoVentaCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().Length(2).WithMessage("El código de planta debe tener 2 caracteres.");
            RuleFor(x => x.Number).NotEmpty().MaximumLength(10).WithMessage("El número de la nota de débito es obligatorio.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Debe indicar el motivo de la anulación.")
                .MaximumLength(500).WithMessage("El motivo no puede exceder los 500 caracteres.");
        }
    }
}
