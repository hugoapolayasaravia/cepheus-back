using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Reportes.Pedidos.GetReportePedidosDetalle;

public sealed class GetReportePedidosDetalleQueryValidator : AbstractValidator<GetReportePedidosDetalleQuery>
{
    public GetReportePedidosDetalleQueryValidator()
    {
        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();
        RuleFor(x => x).Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.");
        RuleFor(x => x.Usuario).MaximumLength(20);
    }
}
