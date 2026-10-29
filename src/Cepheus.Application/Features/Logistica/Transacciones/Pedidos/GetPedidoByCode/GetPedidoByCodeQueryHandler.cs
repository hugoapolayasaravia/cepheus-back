// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/GetPedidoByCode/GetPedidoByCodeQueryHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetPedidoByCode
{
    public class GetPedidoByCodeQueryHandler : IRequestHandler<GetPedidoByCodeQuery, PedidoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetPedidoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PedidoResponse> Handle(GetPedidoByCodeQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var pedido = await _uow.Logistica.Transacciones.Pedidos.Query()
                .AsNoTracking()
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.PlantaCode == plantaCode && p.Code == code, cancellationToken);

            if (pedido is null)
            {
                throw new KeyNotFoundException($"Pedido {plantaCode}/{code} no encontrado.");
            }

            return CreatePedidoCommandHandler.Map(pedido);
        }
    }
}