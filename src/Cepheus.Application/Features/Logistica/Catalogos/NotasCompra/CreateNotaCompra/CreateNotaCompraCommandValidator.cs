using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.CreateNotaCompra
{
    public class CreateNotaCompraCommandValidator : AbstractValidator<CreateNotaCompraCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateNotaCompraCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El texto de la nota es obligatorio.")
                .MaximumLength(200).WithMessage("El texto no puede exceder los 200 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una nota con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.NotasCompra.Query()
                .AnyAsync(n => n.Name == name.Trim().ToUpper(), cancellationToken);
    }
}