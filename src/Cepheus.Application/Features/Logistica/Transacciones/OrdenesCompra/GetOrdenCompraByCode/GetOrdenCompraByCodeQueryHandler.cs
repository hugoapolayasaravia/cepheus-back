// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/GetOrdenCompraByCode/GetOrdenCompraByCodeQueryHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.GetOrdenCompraByCode
{
    public class GetOrdenCompraByCodeQueryHandler : IRequestHandler<GetOrdenCompraByCodeQuery, OrdenCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetOrdenCompraByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraResponse> Handle(GetOrdenCompraByCodeQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var orden = await _uow.Logistica.Transacciones.OrdenesCompra.Query()
                .AsNoTracking()
                .Include(o => o.Detalles).ThenInclude(d => d.Origenes)
                .FirstOrDefaultAsync(o => o.PlantaCode == plantaCode && o.Code == code, cancellationToken);

            if (orden is null)
            {
                throw new KeyNotFoundException($"Orden de Compra {plantaCode}/{code} no encontrada.");
            }

            return OrdenCompraMapper.Map(orden);
        }
    }
}