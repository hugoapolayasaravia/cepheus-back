using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.CreateCotizacionMetradoResumen
{
    public class CreateCotizacionMetradoResumenCommandValidator : AbstractValidator<CreateCotizacionMetradoResumenCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionMetradoResumenCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NegocioCode).NotEmpty().Length(2);
            RuleFor(x => x.Year).NotEmpty().Length(4);
            RuleFor(x => x.Month).NotEmpty().Length(2);
            RuleFor(x => x.Code).NotEmpty().Length(8);
            RuleFor(x => x.LevelNumber).GreaterThan(0);

            RuleFor(x => x)
                .MustAsync(CotizacionEditable)
                .WithMessage("Solo se pueden agregar niveles de metrado a una cotización Pendiente y modificable.");

            RuleFor(x => x)
                .MustAsync(LevelNumberUnique)
                .WithMessage("Ya existe un nivel con ese número en esta cotización.");

            RuleFor(x => x.LevelName).NotEmpty().MaximumLength(50);

            RuleFor(x => x.AlturaLosaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Facturacion.Catalogos.AlturasLosa.Query()
                    .AnyAsync(a => a.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La altura de losa indicada no existe.");

            RuleFor(x => x.OverloadOrShortage).NotNull().MaximumLength(50);
            RuleFor(x => x.BuildingLevel).NotEmpty().MaximumLength(50);

            RuleFor(x => x.LinealMeters).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalVaults).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalMeters).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PricePerM2).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalVaultsAlt).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalMetersAlt).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalPriceAlt).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PricePerM2Alt).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MinTotal).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MinTotalAlt).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MinTotalB).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MinTotalAltB).GreaterThanOrEqualTo(0);
        }

        private async Task<bool> CotizacionEditable(CreateCotizacionMetradoResumenCommand c, CancellationToken ct)
        {
            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstOrDefaultAsync(x => x.NegocioCode == c.NegocioCode.Trim().ToUpper() && x.Year == c.Year
                                        && x.Month == c.Month && x.Code == c.Code.Trim().ToUpper(), ct);

            return cotizacion is not null && cotizacion.IsEditable && cotizacion.Status == EstadoCotizacion.Pendiente;
        }

        private async Task<bool> LevelNumberUnique(CreateCotizacionMetradoResumenCommand c, CancellationToken ct)
            => !await _uow.Facturacion.Transacciones.CotizacionesMetradoResumen.Query()
                .AnyAsync(r => r.NegocioCode == c.NegocioCode.Trim().ToUpper() && r.Year == c.Year
                            && r.Month == c.Month && r.Code == c.Code.Trim().ToUpper() && r.LevelNumber == c.LevelNumber, ct);
    }
}
