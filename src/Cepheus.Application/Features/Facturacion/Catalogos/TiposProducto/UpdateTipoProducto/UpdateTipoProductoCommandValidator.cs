using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.ComprobantesPago.UpdateComprobantePago;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.UpdateUnidadMedidaVenta;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.UpdateTipoProducto
{
    public class UpdateTipoProductoCommandValidator : AbstractValidator<UpdateTipoProductoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoProductoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un tipo de producto con ese código.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro tipo de producto con ese nombre.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueCode(UpdateTipoProductoCommand command, string code, CancellationToken cancellationToken)
             => !await _uow.Facturacion.Catalogos.TiposProducto.Query()
                 .AnyAsync(c => c.Code == code.Trim().ToUpper() && c.Code != command.Code, cancellationToken);


        private async Task<bool> BeUniqueName(UpdateTipoProductoCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.TiposProducto.Query()
                .AnyAsync(t => t.Code != command.Code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

    }
}
