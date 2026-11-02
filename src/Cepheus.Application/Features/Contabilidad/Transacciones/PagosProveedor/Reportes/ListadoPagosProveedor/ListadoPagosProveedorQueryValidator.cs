using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.ListadoPagosProveedor
{
    public class ListadoPagosProveedorQueryValidator : AbstractValidator<ListadoPagosProveedorQuery>
    {
        public ListadoPagosProveedorQueryValidator()
        {
            RuleFor(x => x.FechaDesde).NotNull().WithMessage("La fecha desde es obligatoria.")
                .When(x => string.IsNullOrWhiteSpace(x.Code));

            RuleFor(x => x.FechaHasta).NotNull().WithMessage("La fecha hasta es obligatoria.")
                .When(x => string.IsNullOrWhiteSpace(x.Code));

            RuleFor(x => x)
                .Must(x => !x.FechaDesde.HasValue || !x.FechaHasta.HasValue || x.FechaDesde.Value.Date <= x.FechaHasta.Value.Date)
                .WithMessage("La fecha desde no puede ser mayor a la fecha hasta.");
        }
    }
}
