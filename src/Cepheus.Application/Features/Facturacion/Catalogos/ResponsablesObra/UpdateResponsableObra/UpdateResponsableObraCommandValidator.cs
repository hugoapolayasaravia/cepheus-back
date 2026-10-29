using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.UpdateResponsableObra
{
    public class UpdateResponsableObraCommandValidator : AbstractValidator<UpdateResponsableObraCommand>
    {
        public UpdateResponsableObraCommandValidator()
        {
            RuleFor(x => x.ClienteCode).NotEmpty().MaximumLength(5);
            RuleFor(x => x.ObraCode).NotEmpty().MaximumLength(3);
            RuleFor(x => x.Code).GreaterThan(0);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del responsable es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre del responsable no puede exceder los 50 caracteres.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El teléfono del responsable es obligatorio.")
                .MaximumLength(50).WithMessage("El teléfono del responsable no puede exceder los 50 caracteres.");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("El cargo del responsable es obligatorio.")
                .MaximumLength(50).WithMessage("El cargo del responsable no puede exceder los 50 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
