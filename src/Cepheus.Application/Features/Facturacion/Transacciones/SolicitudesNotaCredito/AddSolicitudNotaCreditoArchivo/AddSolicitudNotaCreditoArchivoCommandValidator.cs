using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.AddSolicitudNotaCreditoArchivo
{
    public class AddSolicitudNotaCreditoArchivoCommandValidator : AbstractValidator<AddSolicitudNotaCreditoArchivoCommand>
    {
        public AddSolicitudNotaCreditoArchivoCommandValidator()
        {
            RuleFor(x => x.Year).NotEmpty().Length(4).WithMessage("El año debe tener 4 caracteres.");
            RuleFor(x => x.Month).NotEmpty().Length(2).WithMessage("El mes debe tener 2 caracteres.");
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código de la solicitud es obligatorio.");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("El nombre del archivo es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre del archivo no puede exceder los 200 caracteres.");

            RuleFor(x => x.FilePath)
                .NotEmpty().WithMessage("La ruta del archivo es obligatoria.")
                .MaximumLength(500).WithMessage("La ruta del archivo no puede exceder los 500 caracteres.");
        }
    }
}
