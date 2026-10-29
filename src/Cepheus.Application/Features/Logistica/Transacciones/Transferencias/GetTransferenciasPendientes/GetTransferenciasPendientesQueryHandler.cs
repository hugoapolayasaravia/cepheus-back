using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Transferencias.GetTransferenciasPendientes;

public sealed class GetTransferenciasPendientesQueryHandler
    : IRequestHandler<GetTransferenciasPendientesQuery, IReadOnlyList<TransferenciaPendienteResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetTransferenciasPendientesQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<TransferenciaPendienteResponse>> Handle(
        GetTransferenciasPendientesQuery request, CancellationToken ct)
    {
        var destino = request.PlantaDestinoCode.Trim().ToUpperInvariant();

        var existe = await _uow.Comunes.Plantas.Query().AsNoTracking().AnyAsync(p => p.Code == destino, ct);
        if (!existe)
            throw new KeyNotFoundException($"La planta {destino} no existe.");

        // Un vale de transferencia no se aprueba ni se procesa en el origen: queda Pendiente hasta que el destino lo recibe.
        var vales = await _uow.Logistica.Transacciones.Vales.Query().AsNoTracking()
            .Where(v => v.PlantaAfectadaCode == destino
                        && v.PlantaCode != destino
                        && v.TipoValeCode == ValeRules.TipoValeTransferencia
                        && v.Estado == EstadoVale.Pendiente)
            .OrderBy(v => v.FechaEntrega).ThenBy(v => v.PlantaCode).ThenBy(v => v.Code)
            .Select(v => new
            {
                v.PlantaCode,
                PlantaName = v.Planta.Name,
                v.Code,
                v.FechaEntrega,
                Lineas = v.Detalles
                    .Where(d => d.Estado == EstadoValeDetalle.Pendiente)
                    .OrderBy(d => d.ItemNumber)
                    .Select(d => new
                    {
                        d.ItemNumber,
                        d.ArticuloCode,
                        ArticuloName = d.Articulo.Name,
                        d.Articulo.UnidadMedidaCode,
                        d.Cantidad,
                        d.Precio,
                        d.Total
                    })
                    .ToList()
            })
            .ToListAsync(ct);

        return vales
            .Where(v => v.Lineas.Count > 0)
            .Select(v => new TransferenciaPendienteResponse(
                v.PlantaCode, v.PlantaName, v.Code, v.FechaEntrega,
                v.Lineas.Sum(l => l.Total),
                v.Lineas.Select(l => new TransferenciaPendienteLineaResponse(
                    l.ItemNumber, l.ArticuloCode, l.ArticuloName, l.UnidadMedidaCode,
                    l.Cantidad, l.Precio, l.Total)).ToList()))
            .ToList();
    }
}
