using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.UpdateClasificacionCliente
{
    public class UpdateClasificacionClienteCommandValidator : AbstractValidator<UpdateClasificacionClienteCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateClasificacionClienteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la clasificación de cliente es obligatorio.")
                .Length(1).WithMessage("El código debe tener 1 carácter.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la clasificación de cliente es obligatorio.")
                .MaximumLength(30).WithMessage("El nombre no puede exceder los 30 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra clasificación de cliente con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateClasificacionClienteCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.ClasificacionesCliente.Query()
                .AnyAsync(t => t.Code != command.Code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
