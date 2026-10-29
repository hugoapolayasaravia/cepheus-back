using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.Common;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.CreateCuentaBancaria
{
    public class CreateCuentaBancariaCommandHandler : IRequestHandler<CreateCuentaBancariaCommand, CuentaBancariaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCuentaBancariaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CuentaBancariaResponse> Handle(CreateCuentaBancariaCommand request, CancellationToken cancellationToken)
        {
            var entity = new CuentaBancaria
            {
                BankCode = request.BankCode.Trim(),
                AccountNumber = request.AccountNumber.Trim(),
                CurrencyCode = request.CurrencyCode.Trim().ToUpper(),
                CheckStart = (request.CheckStart?.Trim() ?? string.Empty),
                CheckEnd = (request.CheckEnd?.Trim() ?? string.Empty),
                CheckCurrent = (request.CheckCurrent?.Trim() ?? string.Empty),
                ControlFlag = request.ControlFlag.Trim().ToUpper(),
                AccountCode = request.AccountCode.Trim(),
                IsActive = true
            };

            await _uow.Contabilidad.Maestros.CuentasBancarias.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static CuentaBancariaResponse Map(CuentaBancaria e) => new()
        {
            BankCode = e.BankCode.TrimEnd(),
            AccountNumber = e.AccountNumber.TrimEnd(),
            CurrencyCode = e.CurrencyCode.TrimEnd(),
            CheckStart = e.CheckStart.TrimEnd(),
            CheckEnd = e.CheckEnd.TrimEnd(),
            CheckCurrent = e.CheckCurrent.TrimEnd(),
            ControlFlag = e.ControlFlag.TrimEnd(),
            AccountCode = e.AccountCode.TrimEnd(),
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
