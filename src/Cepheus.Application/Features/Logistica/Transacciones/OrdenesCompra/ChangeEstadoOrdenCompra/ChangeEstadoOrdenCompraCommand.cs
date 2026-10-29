// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/ChangeEstadoOrdenCompra/ChangeEstadoOrdenCompraCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.ChangeEstadoOrdenCompra
{
    public record ChangeEstadoOrdenCompraCommand(string PlantaCode, string Code, string NuevoEstado) : IRequest<OrdenCompraResponse>;
}