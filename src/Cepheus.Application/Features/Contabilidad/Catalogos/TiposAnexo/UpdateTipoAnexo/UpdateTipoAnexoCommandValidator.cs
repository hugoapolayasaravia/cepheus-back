using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.UpdateTipoAnexo
{
    public class UpdateTipoAnexoCommandValidator : AbstractValidator<UpdateTipoAnexoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoAnexoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.");

            RuleFor(x => x.Flag)
                .MaximumLength(1).WithMessage("El flag no puede exceder 1 caracter.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
