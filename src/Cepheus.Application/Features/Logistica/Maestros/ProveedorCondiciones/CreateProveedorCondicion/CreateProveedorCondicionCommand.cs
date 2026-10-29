using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.CreateProveedorCondicion
{
    public record CreateProveedorCondicionCommand(
        string ProveedorCode,
        string FormaPagoCode,
        int PaymentTermDays,
        string MonedaCode,
        decimal? CreditLimit,
        decimal? DiscountPercentage,
        bool IsPrimary
    ) : IRequest<ProveedorCondicionResponse>;
}
