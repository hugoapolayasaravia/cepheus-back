using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.UpdateLetraVenta
{
    public class UpdateLetraVentaCommandValidator : AbstractValidator<UpdateLetraVentaCommand>
    {
        public UpdateLetraVentaCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().Length(2).WithMessage("El código de planta debe tener 2 caracteres.");
            RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("La versión de la fila es obligatoria.");

            RuleFor(x => x.IssueDate).NotEmpty().WithMessage("Fecha de emisión de la letra errada.");
            RuleFor(x => x.DueDate).NotEmpty().WithMessage("Fecha de vencimiento de la letra errada.");

            RuleFor(x => x.DueDate)
                .Must((cmd, due) => due.Date >= cmd.IssueDate.Date)
                .WithMessage("El vencimiento no puede ser menor a la fecha de emisión.")
                .When(x => x.IssueDate != default && x.DueDate != default);

            RuleFor(x => x.Description).MaximumLength(200).WithMessage("La glosa no puede exceder los 200 caracteres.");
        }
    }
}
