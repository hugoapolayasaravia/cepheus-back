// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/CopyOrdenCompra/CopyOrdenCompraCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.CopyOrdenCompra
{
    public record CopyOrdenCompraCommand(string PlantaCode, string Code, DateTime NuevaFechaEntrega) : IRequest<OrdenCompraResponse>;
}