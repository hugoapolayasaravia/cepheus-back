using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Areas.UpdateArea
{
    public class UpdateAreaCommandValidator : AbstractValidator<UpdateAreaCommand>
    {
        public UpdateAreaCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del área es obligatorio.")
                .MaximumLength(20).WithMessage("El código no puede exceder los 20 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del área es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}