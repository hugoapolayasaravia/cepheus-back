// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/CreateCotizacion/CreateCotizacionCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.CreateCotizacion
{
    public class CreateCotizacionCommandValidator : AbstractValidator<CreateCotizacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(async (code, ct) => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct))
                .WithMessage("La planta indicada no existe.");

            RuleFor(x => x.FechaLimite)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("La fecha límite no puede ser anterior a la fecha del sistema.");

            RuleFor(x => x.Detalles).NotEmpty().WithMessage("La cotización debe tener al menos un artículo a cotizar.");

            RuleForEach(x => x.Detalles).ChildRules(line =>
            {
                line.RuleFor(l => l.ArticuloCode)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("El artículo es obligatorio.")
                    .MustAsync(async (code, ct) => await _uow.Logistica.Maestros.Articulos.Query().AnyAsync(a => a.Code == code.Trim().ToUpper(), ct))
                    .WithMessage("El artículo indicado no existe.");

                line.RuleFor(l => l)
                    .Must(l => l.Origenes is { Count: > 0 } || (l.CantidadArticulo.HasValue && l.CantidadArticulo.Value > 0))
                    .WithMessage("Cada línea debe traer orígenes de Pedido o una cantidad explícita mayor a 0.");
            });

            RuleForEach(x => x.Detalles)
                .ChildRules(line => line.RuleForEach(l => l.Origenes).ChildRules(origen =>
                {
                    origen.RuleFor(o => o.CantidadTomada).GreaterThan(0).WithMessage("La cantidad tomada debe ser mayor a 0.");
                }))
                .When(x => true);

            RuleFor(x => x)
                .MustAsync(PedidoOrigenesExistAndBelongToSamePlanta)
                .WithMessage("Alguna línea de Pedido de origen no existe o no pertenece a la misma planta de la Cotización.");
        }

        private async Task<bool> PedidoOrigenesExistAndBelongToSamePlanta(CreateCotizacionCommand cmd, CancellationToken ct)
        {
            var plantaCode = cmd.PlantaCode.Trim().ToUpperInvariant();

            foreach (var line in cmd.Detalles)
            {
                if (line.Origenes is null) continue;

                foreach (var origen in line.Origenes)
                {
                    var exists = await _uow.Logistica.Transacciones.PedidoDetalles.Query()
                        .AnyAsync(pd => pd.PlantaCode == plantaCode
                                     && pd.PedidoCode == origen.PedidoCode.Trim().ToUpper()
                                     && pd.ItemNumber == origen.PedidoItemNumber, ct);

                    if (!exists) return false;
                }
            }

            return true;
        }
    }
}