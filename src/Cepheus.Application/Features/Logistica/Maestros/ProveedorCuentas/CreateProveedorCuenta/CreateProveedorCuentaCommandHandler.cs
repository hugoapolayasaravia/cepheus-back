using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.CreateProveedorCuenta
{
    public class CreateProveedorCuentaCommandHandler
        : IRequestHandler<CreateProveedorCuentaCommand, ProveedorCuentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateProveedorCuentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorCuentaResponse> Handle(CreateProveedorCuentaCommand request, CancellationToken cancellationToken)
        {
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            if (request.IsPrimary)
            {
                var otras = await _uow.Logistica.Maestros.ProveedorCuentas.Query()
                    .Where(c => c.ProveedorCode == proveedorCode && c.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var otra in otras)
                {
                    otra.IsPrimary = false;
                }
            }

            var cuenta = new ProveedorCuenta
            {
                ProveedorCode = proveedorCode,
                BancoCode = request.BancoCode.Trim().ToUpperInvariant(),
                AccountType = request.AccountType,
                AccountNumber = request.AccountNumber.Trim(),
                InterbankCode = string.IsNullOrWhiteSpace(request.InterbankCode) ? null : request.InterbankCode.Trim(),
                MonedaCode = request.MonedaCode.Trim().ToUpperInvariant(),
                IsPrimary = request.IsPrimary,
                IsActive = true
            };

            await _uow.Logistica.Maestros.ProveedorCuentas.AddAsync(cuenta, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(cuenta);
        }

        internal static ProveedorCuentaResponse Map(ProveedorCuenta cuenta) => new()
        {
            Id = cuenta.Id,
            ProveedorCode = cuenta.ProveedorCode,
            BancoCode = cuenta.BancoCode,
            AccountType = cuenta.AccountType,
            AccountNumber = cuenta.AccountNumber,
            InterbankCode = cuenta.InterbankCode,
            MonedaCode = cuenta.MonedaCode,
            IsPrimary = cuenta.IsPrimary,
            IsActive = cuenta.IsActive,
            CreatedAt = cuenta.CreatedAt,
            UpdatedAt = cuenta.UpdatedAt
        };
    }
}
