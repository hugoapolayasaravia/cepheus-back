using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

public static class OrdenTrabajoVentaMaestros
{
    /// <summary>Cliente y obra existentes y activos (PB: clientes/obras suspendidos o con pedido bloqueado no generan OT).</summary>
    public static async Task<(Cliente Cliente, Obra Obra)> GetActiveClienteObraAsync(
        IUnitOfWork uow,
        string clienteCode,
        string obraCode,
        CancellationToken ct)
    {
        var cliente = await uow.Facturacion.Maestros.Clientes
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == clienteCode, ct)
            ?? throw new KeyNotFoundException($"El cliente {clienteCode} no existe.");

        if (cliente.Estado != EstadoCliente.Activo)
        {
            throw new InvalidOperationException($"El cliente {clienteCode} está {cliente.Estado}; no se puede registrar la Orden de Trabajo.");
        }

        var obra = await OrdenTrabajoVentaCabecera.GetObraAsync(uow, clienteCode, obraCode, ct);

        if (obra.Estado != EstadoObra.Activo)
        {
            throw new InvalidOperationException($"La obra {clienteCode}/{obraCode} está {obra.Estado}; no se puede registrar la Orden de Trabajo.");
        }

        return (cliente, obra);
    }
}
