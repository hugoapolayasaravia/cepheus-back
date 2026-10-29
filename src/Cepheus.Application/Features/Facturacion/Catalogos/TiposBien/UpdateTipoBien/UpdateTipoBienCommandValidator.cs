using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.UpdateTipoBien
{
    public class UpdateTipoBienCommandValidator : AbstractValidator<UpdateTipoBienCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoBienCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatorio.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.");

            RuleFor(x => x.DetractionRate)
                .InclusiveBetween(0m, 100m).WithMessage("La tasa de detracción debe estar entre 0 y 100.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
