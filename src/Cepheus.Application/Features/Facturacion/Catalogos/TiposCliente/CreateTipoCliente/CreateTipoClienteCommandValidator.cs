using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.CreateTipoCliente
{
    public class CreateTipoClienteCommandValidator : AbstractValidator<CreateTipoClienteCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoClienteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de cliente es obligatorio.")
                .MaximumLength(20).WithMessage("El nombre no puede exceder los 20 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de cliente con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.TiposCliente.Query()
                .AnyAsync(t => t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
