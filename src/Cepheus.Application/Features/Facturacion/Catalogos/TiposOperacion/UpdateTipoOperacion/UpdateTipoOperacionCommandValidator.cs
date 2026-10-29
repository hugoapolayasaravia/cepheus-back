using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.UpdateTipoOperacion
{
    public class UpdateTipoOperacionCommandValidator : AbstractValidator<UpdateTipoOperacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoOperacionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatorio.")
                .MaximumLength(255).WithMessage("La descripción no puede exceder los 255 caracteres.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
