using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.Common;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.UpdateCuentaBancaria
{
    public class UpdateCuentaBancariaCommandHandler : IRequestHandler<UpdateCuentaBancariaCommand, CuentaBancariaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCuentaBancariaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CuentaBancariaResponse> Handle(UpdateCuentaBancariaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Contabilidad.Maestros.CuentasBancarias.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BankCode == request.BankCode && x.AccountNumber == request.AccountNumber, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Cuenta bancaria {request.BankCode}/{request.AccountNumber} no encontrado.");
            }

            var entity = new CuentaBancaria
            {
                BankCode = request.BankCode,
                AccountNumber = request.AccountNumber,
                CurrencyCode = request.CurrencyCode.Trim().ToUpper(),
                CheckStart = (request.CheckStart?.Trim() ?? string.Empty),
                CheckEnd = (request.CheckEnd?.Trim() ?? string.Empty),
                CheckCurrent = (request.CheckCurrent?.Trim() ?? string.Empty),
                ControlFlag = request.ControlFlag.Trim().ToUpper(),
                AccountCode = request.AccountCode.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Contabilidad.Maestros.CuentasBancarias.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateCuentaBancaria.CreateCuentaBancariaCommandHandler.Map(entity);
        }
    }
}
