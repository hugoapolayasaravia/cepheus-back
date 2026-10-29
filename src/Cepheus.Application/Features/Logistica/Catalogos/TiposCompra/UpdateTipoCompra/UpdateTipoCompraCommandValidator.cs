using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.UpdateFamilia;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.UpdateTipoCompra
{
    public class UpdateTipoCompraCommandValidator : AbstractValidator<UpdateTipoCompraCommand>
    {
        private readonly IUnitOfWork _uow;
        public UpdateTipoCompraCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del tipo de compra es obligatorio.")
                .Length(1).WithMessage("El código debe tener 1 carácter.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de compra es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra familia con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateTipoCompraCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.TiposCompra.Query()
                .AnyAsync(f => f.Code != command.Code && f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}