using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.UpdateTipoCliente
{
    public class UpdateTipoClienteCommandValidator : AbstractValidator<UpdateTipoClienteCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoClienteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del tipo de cliente es obligatorio.")
                .Length(1).WithMessage("El código debe tener 1 carácter.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de cliente es obligatorio.")
                .MaximumLength(20).WithMessage("El nombre no puede exceder los 20 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro tipo de cliente con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateTipoClienteCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.TiposCliente.Query()
                .AnyAsync(t => t.Code != command.Code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
