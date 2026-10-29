using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.CreateCuentaBancaria;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.GetCuentaBancariaByCode
{
    public class GetCuentaBancariaByCodeQueryHandler : IRequestHandler<GetCuentaBancariaByCodeQuery, CuentaBancariaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCuentaBancariaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CuentaBancariaResponse> Handle(GetCuentaBancariaByCodeQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Maestros.CuentasBancarias.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BankCode == request.BankCode && x.AccountNumber == request.AccountNumber, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Cuenta bancaria {request.BankCode}/{request.AccountNumber} no encontrado.");
            }

            return CreateCuentaBancariaCommandHandler.Map(entity);
        }
    }
}
