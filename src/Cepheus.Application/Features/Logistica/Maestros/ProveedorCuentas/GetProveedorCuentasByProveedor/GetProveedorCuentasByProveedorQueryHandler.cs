using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.GetProveedorCuentasByProveedor
{
    public class GetProveedorCuentasByProveedorQueryHandler
        : IRequestHandler<GetProveedorCuentasByProveedorQuery, List<ProveedorCuentaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorCuentasByProveedorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ProveedorCuentaResponse>> Handle(
            GetProveedorCuentasByProveedorQuery request, CancellationToken cancellationToken)
        {
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            return await _uow.Logistica.Maestros.ProveedorCuentas.Query()
                .AsNoTracking()
                .Where(c => c.ProveedorCode == proveedorCode)
                .OrderByDescending(c => c.IsPrimary)
                .ThenBy(c => c.Id)
                .Select(c => new ProveedorCuentaResponse
                {
                    Id = c.Id,
                    ProveedorCode = c.ProveedorCode,
                    BancoCode = c.BancoCode,
                    AccountType = c.AccountType,
                    AccountNumber = c.AccountNumber,
                    InterbankCode = c.InterbankCode,
                    MonedaCode = c.MonedaCode,
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
