// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/GetOrdenCompraByCode/GetOrdenCompraByCodeQuery.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.GetOrdenCompraByCode
{
    public record GetOrdenCompraByCodeQuery(string PlantaCode, string Code) : IRequest<OrdenCompraResponse>;
}