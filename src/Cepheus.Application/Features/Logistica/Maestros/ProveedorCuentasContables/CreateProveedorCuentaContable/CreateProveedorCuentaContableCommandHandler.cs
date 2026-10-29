using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.CreateProveedorCuentaContable
{
    public class CreateProveedorCuentaContableCommandHandler
        : IRequestHandler<CreateProveedorCuentaContableCommand, ProveedorCuentaContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateProveedorCuentaContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorCuentaContableResponse> Handle(
            CreateProveedorCuentaContableCommand request, CancellationToken cancellationToken)
        {
            var entity = new ProveedorCuentaContable
            {
                ProveedorCode = request.ProveedorCode.Trim().ToUpperInvariant(),
                MonedaCode = request.MonedaCode.Trim().ToUpperInvariant(),
                AccountingAccountCode = request.AccountingAccountCode.Trim().ToUpperInvariant()
            };

            await _uow.Logistica.Maestros.ProveedorCuentasContables.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ProveedorCuentaContableMapper.Map(entity);
        }
    }
}
