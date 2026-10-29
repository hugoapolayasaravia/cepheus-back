using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.UpdateNotaCreditoVenta
{
    public class UpdateNotaCreditoVentaCommandValidator : AbstractValidator<UpdateNotaCreditoVentaCommand>
    {
        public UpdateNotaCreditoVentaCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().Length(2).WithMessage("El código de planta debe tener 2 caracteres.");
            RuleFor(x => x.Number).NotEmpty().MaximumLength(10).WithMessage("El número de la nota de crédito es obligatorio.");
            RuleFor(x => x.IssueDate).NotEmpty().WithMessage("La fecha de emisión es obligatoria.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La glosa es obligatoria.")
                .MaximumLength(200).WithMessage("La glosa no puede exceder los 200 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
