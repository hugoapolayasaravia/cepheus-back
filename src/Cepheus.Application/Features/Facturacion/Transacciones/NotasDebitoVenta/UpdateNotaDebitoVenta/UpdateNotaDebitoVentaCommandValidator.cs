using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.UpdateNotaDebitoVenta
{
    public class UpdateNotaDebitoVentaCommandValidator : AbstractValidator<UpdateNotaDebitoVentaCommand>
    {
        public UpdateNotaDebitoVentaCommandValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.PlantaCode).NotEmpty().Length(2).WithMessage("El código de planta debe tener 2 caracteres.");
            RuleFor(x => x.Number).NotEmpty().MaximumLength(10).WithMessage("El número de la nota de débito es obligatorio.");

            RuleFor(x => x.IssueDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La fecha de emisión es obligatoria.")
                .Must(d => d.Date <= DateTime.Today).WithMessage("La fecha de emisión no puede ser futura.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La glosa es obligatoria.")
                .MaximumLength(200).WithMessage("La glosa no puede exceder los 200 caracteres.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("La versión de la nota es obligatoria.");

            RuleFor(x => x.Details)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Debe registrar al menos una línea de detalle.")
                .Must(d => d.Count <= NotaDebitoVentaRules.MaxLines)
                    .WithMessage($"El detalle no puede exceder las {NotaDebitoVentaRules.MaxLines} líneas.")
                .Must(NotaDebitoVentaDetailsValidation.NoDuplicateLines)
                    .WithMessage("Hay líneas duplicadas (mismo producto y clave de línea).")
                .MustAsync((lines, ct) => NotaDebitoVentaDetailsValidation.ProductsExistAsync(uow, lines, ct))
                    .WithMessage("Alguno de los productos indicados no existe.");

            RuleForEach(x => x.Details).SetValidator(new NotaDebitoVentaDetalleInputValidator());
        }
    }
}
