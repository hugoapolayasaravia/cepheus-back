// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/CreatePedido/CreatePedidoCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido
{
    public class CreatePedidoCommandValidator : AbstractValidator<CreatePedidoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreatePedidoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.TipoPedidoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de pedido es obligatorio.")
                .MustAsync(TipoPedidoExists).WithMessage("El tipo de pedido indicado no existe.");

            RuleFor(x => x.TipoValeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Pedido por es obligatorio.")
                .MustAsync(TipoValeExists)
                .WithMessage("Pedido por indicado no existe.");

            RuleFor(x => x.TramiteCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El trámite es obligatorio.")
                .MustAsync(TramiteExists).WithMessage("El trámite indicado no existe.");

            RuleFor(x => x.SubCentroCostoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El subcentro de costo es obligatorio.")
                .MustAsync(SubCentroCostoExists).WithMessage("El subcentro de costo indicado no existe.");

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El trabajador solicitante es obligatorio.")
                .MustAsync(TrabajadorExists).WithMessage("El trabajador indicado no existe.");

            RuleFor(x => x)
                .MustAsync(OrdenTrabajoExists)
                .WithMessage("La Orden de Trabajo indicada no existe en esa planta.")
                .When(x => !string.IsNullOrWhiteSpace(x.OrdenTrabajoCode));

            RuleFor(x => x.UnidadNegocioCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La unidad de negocio es obligatoria.")
                .MustAsync(UnidadNegocioExists).WithMessage("La unidad de negocio indicada no existe.");

            RuleFor(x => x.FechaEntrega)
                .LessThanOrEqualTo(DateTime.Today.AddDays(1).AddTicks(-1))
                .WithMessage("La fecha de entrega no puede ser mayor a la fecha del sistema.");

            RuleFor(x => x.Detalles)
                .NotEmpty().WithMessage("El pedido debe tener al menos una línea de detalle.");

            RuleForEach(x => x.Detalles).ChildRules(line =>
            {
                line.RuleFor(l => l.DescripcionArticulo)
                    .NotEmpty().WithMessage("La descripción del artículo es obligatoria.")
                    .MaximumLength(80).WithMessage("La descripción no puede exceder los 80 caracteres.");

                line.RuleFor(l => l.CantidadArticulo)
                    .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

                line.RuleFor(l => l.PrecioArticulo)
                    .GreaterThanOrEqualTo(0).WithMessage("El precio no puede ser negativo.");

                line.RuleFor(l => l.UnidadMedidaCode)
                    .NotEmpty().WithMessage("La unidad de medida es obligatoria.")
                    .MustAsync(UnidadMedidaExists).WithMessage("La unidad de medida indicada no existe.");

                line.RuleFor(l => l.ProveedorCode)
                    .MustAsync(ProveedorExists).WithMessage("El proveedor indicado no existe.")
                    .When(l => !string.IsNullOrWhiteSpace(l.ProveedorCode));

                // ArticuloCode: sin validación de existencia a propósito — puede ser
                // una línea de descripción libre sin artículo real detrás (ver
                // comentario en la entidad PedidoDetalle).
            });
        }

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> TipoPedidoExists(string code, CancellationToken ct)
            => await _uow.Logistica.Catalogos.TiposPedido.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> TipoValeExists(string? code, CancellationToken ct)
            => await _uow.Logistica.Catalogos.TiposVale.Query().AnyAsync(t => t.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> TramiteExists(string code, CancellationToken ct)
            => await _uow.Logistica.Catalogos.Tramites.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> SubCentroCostoExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.SubCentrosCosto.Query().AnyAsync(s => s.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> TrabajadorExists(string code, CancellationToken ct)
            => await _uow.Rrhh.Maestros.Trabajadores.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> OrdenTrabajoExists(CreatePedidoCommand cmd, CancellationToken ct)
            => await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query()
                .AnyAsync(o => o.PlantaCode == cmd.PlantaCode.Trim().ToUpper()
                             && o.Code == cmd.OrdenTrabajoCode!.Trim().ToUpper(), ct);

        private async Task<bool> UnidadNegocioExists(string code, CancellationToken ct)
            => await _uow.Logistica.Catalogos.UnidadesNegocio.Query().AnyAsync(u => u.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> UnidadMedidaExists(string code, CancellationToken ct)
            => await _uow.Logistica.Catalogos.UnidadesMedida.Query().AnyAsync(u => u.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ProveedorExists(string? code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Proveedores.Query().AnyAsync(p => p.Code == code!.Trim().ToUpper(), ct);
    }
}