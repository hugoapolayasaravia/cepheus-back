// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/GetOrdenCompraAdministracionByCode/GetOrdenCompraAdministracionByCodeQueryHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.GetOrdenCompraAdmByCode
{
    public class GetOrdenCompraAdmByCodeQueryHandler
        : IRequestHandler<GetOrdenCompraAdmByCodeQuery, OrdenCompraAdmResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetOrdenCompraAdmByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraAdmResponse> Handle(
            GetOrdenCompraAdmByCodeQuery request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim().ToUpperInvariant();

            var orden = await _uow.Logistica.Transacciones.OrdenesCompraAdministracion.Query()
                .AsNoTracking()
                .Include(o => o.Proveedor)
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Code == code, cancellationToken);

            if (orden is null)
            {
                throw new KeyNotFoundException($"Orden de Compra de Administración {code} no encontrada.");
            }

            return OrdenCompraAdmMapper.Map(orden);
        }
    }
}
