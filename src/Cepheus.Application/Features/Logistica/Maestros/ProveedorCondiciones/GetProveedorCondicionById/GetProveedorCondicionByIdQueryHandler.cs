using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.GetProveedorCondicionById
{
    public class GetProveedorCondicionByIdQueryHandler : IRequestHandler<GetProveedorCondicionByIdQuery, ProveedorCondicionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorCondicionByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorCondicionResponse> Handle(GetProveedorCondicionByIdQuery request, CancellationToken cancellationToken)
        {
            var condicion = await _uow.Logistica.Maestros.ProveedorCondiciones.Query()
                .AsNoTracking()
                .Where(c => c.Id == request.Id)
                .Select(c => new ProveedorCondicionResponse
                {
                    Id = c.Id,
                    ProveedorCode = c.ProveedorCode,
                    FormaPagoCode = c.FormaPagoCode,
                    PaymentTermDays = c.PaymentTermDays,
                    MonedaCode = c.MonedaCode,
                    CreditLimit = c.CreditLimit,
                    DiscountPercentage = c.DiscountPercentage,
                    IsPrimary = c.IsPrimary,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (condicion is null)
            {
                throw new KeyNotFoundException($"Condición {request.Id} no encontrada.");
            }

            return condicion;
        }
    }
}
