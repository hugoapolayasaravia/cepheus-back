using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.GetProveedorCondicionesByProveedor
{
    public class GetProveedorCondicionesByProveedorQueryHandler
        : IRequestHandler<GetProveedorCondicionesByProveedorQuery, List<ProveedorCondicionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorCondicionesByProveedorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ProveedorCondicionResponse>> Handle(
            GetProveedorCondicionesByProveedorQuery request, CancellationToken cancellationToken)
        {
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            return await _uow.Logistica.Maestros.ProveedorCondiciones.Query()
                .AsNoTracking()
                .Where(c => c.ProveedorCode == proveedorCode)
                .OrderByDescending(c => c.IsPrimary)
                .ThenBy(c => c.Id)
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
                    UpdatedAt = c.UpdatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedBy = c.UpdatedBy
                })
                .ToListAsync(cancellationToken);
        }
    }
}
