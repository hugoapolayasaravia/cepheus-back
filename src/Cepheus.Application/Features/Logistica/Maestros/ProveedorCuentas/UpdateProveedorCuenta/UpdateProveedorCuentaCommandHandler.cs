using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.UpdateProveedorCuenta
{
    public class UpdateProveedorCuentaCommandHandler
        : IRequestHandler<UpdateProveedorCuentaCommand, ProveedorCuentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProveedorCuentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorCuentaResponse> Handle(UpdateProveedorCuentaCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _uow.Logistica.Maestros.ProveedorCuentas.GetByIdAsync(request.Id, cancellationToken);

            if (cuenta is null)
            {
                throw new KeyNotFoundException($"Cuenta {request.Id} no encontrada.");
            }

            if (request.IsPrimary && !cuenta.IsPrimary)
            {
                var otras = await _uow.Logistica.Maestros.ProveedorCuentas.Query()
                    .Where(c => c.ProveedorCode == cuenta.ProveedorCode && c.IsPrimary && c.Id != cuenta.Id)
                    .ToListAsync(cancellationToken);

                foreach (var otra in otras)
                {
                    otra.IsPrimary = false;
                }
            }

            cuenta.BancoCode = request.BancoCode.Trim().ToUpperInvariant();
            cuenta.AccountType = request.AccountType;
            cuenta.AccountNumber = request.AccountNumber.Trim();
            cuenta.InterbankCode = string.IsNullOrWhiteSpace(request.InterbankCode) ? null : request.InterbankCode.Trim();
            cuenta.MonedaCode = request.MonedaCode.Trim().ToUpperInvariant();
            cuenta.IsPrimary = request.IsPrimary;

            await _uow.SaveChangesAsync(cancellationToken);

            return CreateProveedorCuenta.CreateProveedorCuentaCommandHandler.Map(cuenta);
        }
    }
}
