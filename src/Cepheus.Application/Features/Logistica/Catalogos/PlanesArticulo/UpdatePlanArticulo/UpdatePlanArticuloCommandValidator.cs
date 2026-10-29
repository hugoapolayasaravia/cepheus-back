using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.UpdatePlanArticulo
{
    public class UpdatePlanArticuloCommandValidator : AbstractValidator<UpdatePlanArticuloCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePlanArticuloCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del plan es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del plan es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro plan con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdatePlanArticuloCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.PlanesArticulo.Query()
                .AnyAsync(p => p.Code != command.Code && p.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}