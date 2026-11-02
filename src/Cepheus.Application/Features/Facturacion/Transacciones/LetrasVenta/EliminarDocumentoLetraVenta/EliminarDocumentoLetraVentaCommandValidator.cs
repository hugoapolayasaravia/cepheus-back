using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.EliminarDocumentoLetraVenta
{
    public class EliminarDocumentoLetraVentaCommandValidator : AbstractValidator<EliminarDocumentoLetraVentaCommand>
    {
        public EliminarDocumentoLetraVentaCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().Length(2);
            RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Item).NotEmpty().MaximumLength(3);
        }
    }
}
