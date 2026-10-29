using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.GetProveedorCuentaContableByKey
{
    public class GetProveedorCuentaContableByKeyQueryHandler
        : IRequestHandler<GetProveedorCuentaContableByKeyQuery, ProveedorCuentaContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorCuentaContableByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorCuentaContableResponse> Handle(
            GetProveedorCuentaContableByKeyQuery request, CancellationToken cancellationToken)
        {
            var prv = request.ProveedorCode.Trim().ToUpperInvariant();
            var mon = request.MonedaCode.Trim().ToUpperInvariant();

            var entity = await _uow.Logistica.Maestros.ProveedorCuentasContables.Query()
                .AsNoTracking()
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(c => c.ProveedorCode == prv && c.MonedaCode == mon, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"El proveedor {prv} no tiene cuenta por pagar para la moneda {mon}.");
            }

            return ProveedorCuentaContableMapper.Map(entity);
        }
    }
}
