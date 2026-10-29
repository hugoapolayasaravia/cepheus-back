using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.CreatePlanArticulo
{
    public class CreatePlanArticuloCommandValidator : AbstractValidator<CreatePlanArticuloCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreatePlanArticuloCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del plan es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un plan con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.PlanesArticulo.Query()
                .AnyAsync(p => p.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}