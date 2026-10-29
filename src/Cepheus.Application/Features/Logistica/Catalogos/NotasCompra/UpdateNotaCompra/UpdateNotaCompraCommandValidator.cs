using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.UpdateNotaCompra
{
    public class UpdateNotaCompraCommandValidator : AbstractValidator<UpdateNotaCompraCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNotaCompraCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la nota es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El texto de la nota es obligatorio.")
                .MaximumLength(200).WithMessage("El texto no puede exceder los 200 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra familia con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateNotaCompraCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.NotasCompra.Query()
                .AnyAsync(f => f.Code != command.Code && f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}