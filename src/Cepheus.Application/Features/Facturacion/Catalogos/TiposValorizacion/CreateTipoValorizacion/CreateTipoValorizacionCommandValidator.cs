using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.CreateTipoValorizacion
{
    public class CreateTipoValorizacionCommandValidator : AbstractValidator<CreateTipoValorizacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoValorizacionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de valorización es obligatorio.")
                .MaximumLength(10).WithMessage("El nombre no puede exceder los 10 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de valorización con ese nombre.");

            RuleFor(x => x.Days)
                .InclusiveBetween(0, 366).WithMessage("Los días de valorización deben estar entre 0 y 366.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.TiposValorizacion.Query()
                .AnyAsync(t => t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
