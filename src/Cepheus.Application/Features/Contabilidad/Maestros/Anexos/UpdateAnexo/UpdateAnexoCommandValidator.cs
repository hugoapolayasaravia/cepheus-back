using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.UpdateAnexo
{
    public class UpdateAnexoCommandValidator : AbstractValidator<UpdateAnexoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAnexoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TipoAnexoCode)
                .NotEmpty().WithMessage("El tipo de anexo es obligatorio.")
                .Length(3).WithMessage("El tipo de anexo debe tener 3 caracteres.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(5).WithMessage("El código debe tener 5 caracteres.");

            RuleFor(x => x.Name)
                .MaximumLength(150).WithMessage("La descripción no puede exceder los 150 caracteres.");

            RuleFor(x => x.LinkAccount1Code)
                .MaximumLength(8).WithMessage("La cuenta de enlace 1 no puede exceder los 8 caracteres.");

            RuleFor(x => x.LinkAccount2Code)
                .MaximumLength(8).WithMessage("La cuenta de enlace 2 no puede exceder los 8 caracteres.");

            RuleFor(x => x.Flag)
                .MaximumLength(1).WithMessage("El flag no puede exceder 1 carácter.");
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
