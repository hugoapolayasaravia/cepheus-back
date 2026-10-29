// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/GetPedidoByCode/GetPedidoByCodeQuery.cs
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetPedidoByCode
{
    public record GetPedidoByCodeQuery(string PlantaCode, string Code) : IRequest<PedidoResponse>;
}