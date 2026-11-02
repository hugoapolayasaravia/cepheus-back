using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.ProtestarLetraVenta
{
    public class ProtestarLetraVentaCommandValidator : AbstractValidator<ProtestarLetraVentaCommand>
    {
        public ProtestarLetraVentaCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().Length(2).WithMessage("El código de planta debe tener 2 caracteres.");
            RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("La versión de la fila es obligatoria.");
        }
    }
}
