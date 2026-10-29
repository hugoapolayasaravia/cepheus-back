using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.UpdateFlete
{
    public class UpdateFleteCommandValidator : AbstractValidator<UpdateFleteCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateFleteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del flete es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del flete es obligatorio.")
                .MaximumLength(20).WithMessage("El nombre no puede exceder los 20 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro flete con ese nombre.");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("El monto del flete no puede ser negativo.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateFleteCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.Fletes.Query()
                .AnyAsync(t => t.Code != command.Code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
