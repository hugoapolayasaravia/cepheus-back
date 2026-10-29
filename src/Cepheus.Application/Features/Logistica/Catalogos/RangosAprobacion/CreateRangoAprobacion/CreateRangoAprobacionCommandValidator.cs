using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.CreateRangoAprobacion
{
    public class CreateRangoAprobacionCommandValidator : AbstractValidator<CreateRangoAprobacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateRangoAprobacionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NivelCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nivel es obligatorio.")
                .MustAsync(NivelExists).WithMessage("El nivel indicado no existe.");

            RuleFor(x => x.TipoTransaccionCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de transacción es obligatorio.")
                .MustAsync(TipoTransaccionExists).WithMessage("El tipo de transacción indicado no existe.");

            RuleFor(x => x.UnidadNegocioCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La unidad de negocio es obligatoria.")
                .MustAsync(UnidadNegocioExists).WithMessage("La unidad de negocio indicada no existe.");

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .MustAsync(MonedaExists).WithMessage("La moneda indicada no existe.");

            RuleFor(x => x)
                .MustAsync(BeUniqueCombination)
                .WithMessage("Ya existe un rango de aprobación para esa combinación de nivel, transacción, unidad de negocio y moneda.")
                .OverridePropertyName(nameof(CreateRangoAprobacionCommand.NivelCode));

            RuleFor(x => x.ImporteMinimo).GreaterThanOrEqualTo(0).WithMessage("El importe mínimo no puede ser negativo.");

            RuleFor(x => x.ImporteMaximo)
                .GreaterThanOrEqualTo(x => x.ImporteMinimo)
                .WithMessage("El importe máximo no puede ser menor al importe mínimo.");

            RuleFor(x => x.ImporteAcumuladoDiario).GreaterThanOrEqualTo(0).WithMessage("El acumulado diario no puede ser negativo.");
            RuleFor(x => x.ImporteAcumuladoMensual).GreaterThanOrEqualTo(0).WithMessage("El acumulado mensual no puede ser negativo.");

            RuleFor(x => x.PorcentajeTotal)
                .InclusiveBetween(0, 100).WithMessage("El porcentaje total debe estar entre 0 y 100.")
                .When(x => x.PorcentajeTotal.HasValue);
        }

        private async Task<bool> NivelExists(string code, CancellationToken ct)
            => await _uow.Logistica.Catalogos.Niveles.Query().AnyAsync(n => n.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> TipoTransaccionExists(string code, CancellationToken ct)
            => await _uow.Logistica.Catalogos.TiposTransaccion.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> UnidadNegocioExists(string code, CancellationToken ct)
            => await _uow.Logistica.Catalogos.UnidadesNegocio.Query().AnyAsync(u => u.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> MonedaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> BeUniqueCombination(CreateRangoAprobacionCommand command, CancellationToken ct)
        {
            var nivel = command.NivelCode.Trim().ToUpper();
            var trans = command.TipoTransaccionCode.Trim().ToUpper();
            var une = command.UnidadNegocioCode.Trim().ToUpper();
            var mon = command.MonedaCode.Trim().ToUpper();

            return !await _uow.Logistica.Catalogos.RangosAprobacion.Query()
                .AnyAsync(r => r.NivelCode == nivel && r.TipoTransaccionCode == trans &&
                               r.UnidadNegocioCode == une && r.MonedaCode == mon, ct);
        }
    }
}
