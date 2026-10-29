using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.UpdateTipoValorizacion
{
    public class UpdateTipoValorizacionCommandValidator : AbstractValidator<UpdateTipoValorizacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoValorizacionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del tipo de valorización es obligatorio.")
                .Length(1).WithMessage("El código debe tener 1 carácter.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de valorización es obligatorio.")
                .MaximumLength(10).WithMessage("El nombre no puede exceder los 10 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro tipo de valorización con ese nombre.");

            RuleFor(x => x.Days)
                .InclusiveBetween(0, 366).WithMessage("Los días de valorización deben estar entre 0 y 366.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateTipoValorizacionCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.TiposValorizacion.Query()
                .AnyAsync(t => t.Code != command.Code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
