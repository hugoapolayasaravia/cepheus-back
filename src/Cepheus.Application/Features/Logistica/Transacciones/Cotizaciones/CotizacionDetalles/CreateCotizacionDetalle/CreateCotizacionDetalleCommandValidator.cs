// Cepheus.Application/Features/Logistica/Transacciones/CotizacionDetalles/CreateCotizacionDetalle/CreateCotizacionDetalleCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionDetalles.CreateCotizacionDetalle
{
    public class CreateCotizacionDetalleCommandValidator : AbstractValidator<CreateCotizacionDetalleCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionDetalleCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.CotizacionCode).NotEmpty();

            RuleFor(x => x.ArticuloCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El artículo es obligatorio.")
                .MustAsync(async (code, ct) => await _uow.Logistica.Maestros.Articulos.Query().AnyAsync(a => a.Code == code.Trim().ToUpper(), ct))
                .WithMessage("El artículo indicado no existe.");

            RuleFor(x => x)
                .Must(x => x.Origenes is { Count: > 0 } || (x.CantidadArticulo.HasValue && x.CantidadArticulo.Value > 0))
                .WithMessage("Debe traer orígenes de Pedido o una cantidad explícita mayor a 0.");

            RuleForEach(x => x.Origenes).ChildRules(o =>
                o.RuleFor(x => x.CantidadTomada).GreaterThan(0).WithMessage("La cantidad tomada debe ser mayor a 0."));

            RuleFor(x => x)
                .MustAsync(PedidoOrigenesExist)
                .WithMessage("Alguna línea de Pedido de origen no existe en esa planta.")
                .When(x => x.Origenes is { Count: > 0 });
        }

        private async Task<bool> PedidoOrigenesExist(CreateCotizacionDetalleCommand cmd, CancellationToken ct)
        {
            var plantaCode = cmd.PlantaCode.Trim().ToUpperInvariant();
            foreach (var origen in cmd.Origenes!)
            {
                var exists = await _uow.Logistica.Transacciones.PedidoDetalles.Query()
                    .AnyAsync(pd => pd.PlantaCode == plantaCode
                                 && pd.PedidoCode == origen.PedidoCode.Trim().ToUpper()
                                 && pd.ItemNumber == origen.PedidoItemNumber, ct);
                if (!exists) return false;
            }
            return true;
        }
    }
}