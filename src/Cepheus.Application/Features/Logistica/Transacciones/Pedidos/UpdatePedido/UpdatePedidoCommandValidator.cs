// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/UpdatePedido/UpdatePedidoCommandValidator.cs
// Mismas reglas de existencia que CreatePedidoCommandValidator, reutilizadas contra los mismos catálogos.
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.UpdatePedido
{
    public class UpdatePedidoCommandValidator : AbstractValidator<UpdatePedidoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePedidoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();

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

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para concurrencia.");
        }

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

        private async Task<bool> OrdenTrabajoExists(UpdatePedidoCommand cmd, CancellationToken ct)
            => await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query()
                .AnyAsync(o => o.PlantaCode == cmd.PlantaCode.Trim().ToUpper()
                             && o.Code == cmd.OrdenTrabajoCode!.Trim().ToUpper(), ct);

        private async Task<bool> UnidadNegocioExists(string code, CancellationToken ct)
            => await _uow.Logistica.Catalogos.UnidadesNegocio.Query().AnyAsync(u => u.Code == code.Trim().ToUpper(), ct);
    }
}