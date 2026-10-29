using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.CreateCuentaContable;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.GetCuentaContableByCode
{
    public class GetCuentaContableByCodeQueryHandler : IRequestHandler<GetCuentaContableByCodeQuery, CuentaContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCuentaContableByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CuentaContableResponse> Handle(GetCuentaContableByCodeQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Maestros.CuentasContables.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Cuenta contable {request.Code} no encontrado.");
            }

            return CreateCuentaContableCommandHandler.Map(entity);
        }
    }
}
