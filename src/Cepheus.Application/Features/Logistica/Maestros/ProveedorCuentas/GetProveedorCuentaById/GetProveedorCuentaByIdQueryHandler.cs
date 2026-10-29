using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.GetProveedorCuentaById
{
    public class GetProveedorCuentaByIdQueryHandler : IRequestHandler<GetProveedorCuentaByIdQuery, ProveedorCuentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorCuentaByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorCuentaResponse> Handle(GetProveedorCuentaByIdQuery request, CancellationToken cancellationToken)
        {
            var cuenta = await _uow.Logistica.Maestros.ProveedorCuentas.Query()
                .AsNoTracking()
                .Where(c => c.Id == request.Id)
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
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (cuenta is null)
            {
                throw new KeyNotFoundException($"Cuenta {request.Id} no encontrada.");
            }

            return cuenta;
        }
    }
}
