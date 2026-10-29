using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.CreateUnidadMedidaVenta
{
    public class CreateUnidadMedidaVentaCommandValidator : AbstractValidator<CreateUnidadMedidaVentaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateUnidadMedidaVentaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un registro con ese código.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una unidad de medida con ese nombre.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken ct)
            => !await _uow.Facturacion.Catalogos.UnidadesMedidaVenta.Query().AnyAsync(x => x.Code == code.Trim().ToUpper(), ct);


        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.UnidadesMedidaVenta.Query()
                .AnyAsync(t => t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
