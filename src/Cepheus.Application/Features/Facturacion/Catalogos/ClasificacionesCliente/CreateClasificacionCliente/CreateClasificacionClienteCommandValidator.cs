using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.CreateClasificacionCliente
{
    public class CreateClasificacionClienteCommandValidator : AbstractValidator<CreateClasificacionClienteCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateClasificacionClienteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la clasificación de cliente es obligatorio.")
                .MaximumLength(30).WithMessage("El nombre no puede exceder los 30 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una clasificación de cliente con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.ClasificacionesCliente.Query()
                .AnyAsync(t => t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
