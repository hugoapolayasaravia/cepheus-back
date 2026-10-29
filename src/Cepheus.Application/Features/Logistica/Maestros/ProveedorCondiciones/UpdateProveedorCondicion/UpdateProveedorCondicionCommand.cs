using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.UpdateProveedorCondicion
{
    public record UpdateProveedorCondicionCommand(
        int Id,
        string FormaPagoCode,
        int PaymentTermDays,
        string MonedaCode,
        decimal? CreditLimit,
        decimal? DiscountPercentage,
        bool IsPrimary
    ) : IRequest<ProveedorCondicionResponse>;
}
