using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.CreateCotizacionNota
{
    public class CreateCotizacionNotaCommandValidator : AbstractValidator<CreateCotizacionNotaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionNotaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NegocioCode).NotEmpty().Length(2);
            RuleFor(x => x.Year).NotEmpty().Length(4);
            RuleFor(x => x.Month).NotEmpty().Length(2);
            RuleFor(x => x.Code).NotEmpty().Length(8);
            RuleFor(x => x.Description).NotEmpty().WithMessage("La descripción de la nota es obligatoria.");
            RuleFor(x => x.Option).IsInEnum();

            RuleFor(x => x)
                .MustAsync(CotizacionExists)
                .WithMessage("La cotización indicada no existe.");
        }

        private async Task<bool> CotizacionExists(CreateCotizacionNotaCommand c, CancellationToken ct)
            => await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .AnyAsync(x => x.NegocioCode == c.NegocioCode.Trim().ToUpper() && x.Year == c.Year
                            && x.Month == c.Month && x.Code == c.Code.Trim().ToUpper(), ct);
    }
}
