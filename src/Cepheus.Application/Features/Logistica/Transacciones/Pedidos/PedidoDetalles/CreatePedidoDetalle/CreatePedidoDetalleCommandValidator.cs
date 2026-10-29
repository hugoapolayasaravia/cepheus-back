// Cepheus.Application/Features/Logistica/Transacciones/PedidoDetalles/CreatePedidoDetalle/CreatePedidoDetalleCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.CreatePedidoDetalle
{
    public class CreatePedidoDetalleCommandValidator : AbstractValidator<CreatePedidoDetalleCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreatePedidoDetalleCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.PedidoCode).NotEmpty();

            RuleFor(x => x.DescripcionArticulo)
                .NotEmpty().WithMessage("La descripción del artículo es obligatoria.")
                .MaximumLength(80).WithMessage("La descripción no puede exceder los 80 caracteres.");

            RuleFor(x => x.CantidadArticulo).GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");
            RuleFor(x => x.PrecioArticulo).GreaterThanOrEqualTo(0).WithMessage("El precio no puede ser negativo.");

            RuleFor(x => x.UnidadMedidaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La unidad de medida es obligatoria.")
                .MustAsync(UnidadMedidaExists).WithMessage("La unidad de medida indicada no existe.");

            RuleFor(x => x.ProveedorCode)
                .MustAsync(ProveedorExists).WithMessage("El proveedor indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ProveedorCode));
        }

        private async Task<bool> UnidadMedidaExists(string code, CancellationToken ct)
            => await _uow.Logistica.Catalogos.UnidadesMedida.Query().AnyAsync(u => u.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ProveedorExists(string? code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Proveedores.Query().AnyAsync(p => p.Code == code!.Trim().ToUpper(), ct);
    }
}