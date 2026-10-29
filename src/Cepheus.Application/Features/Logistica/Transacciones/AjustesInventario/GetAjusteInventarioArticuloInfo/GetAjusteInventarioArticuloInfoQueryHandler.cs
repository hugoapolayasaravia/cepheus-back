using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.GetAjusteInventarioArticuloInfo;

public sealed class GetAjusteInventarioArticuloInfoQueryHandler
    : IRequestHandler<GetAjusteInventarioArticuloInfoQuery, AjusteInventarioArticuloInfoResponse>
{
    private readonly IUnitOfWork _uow;

    public GetAjusteInventarioArticuloInfoQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<AjusteInventarioArticuloInfoResponse> Handle(
        GetAjusteInventarioArticuloInfoQuery request,
        CancellationToken ct)
    {
        var planta = AjusteInventarioRules.Normalize(request.PlantaCode);
        var articuloCode = AjusteInventarioRules.Normalize(request.ArticuloCode);

        var articulo = await _uow.Logistica.Maestros.Articulos
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Code == articuloCode, ct)
            ?? throw new KeyNotFoundException($"El artículo {articuloCode} no existe.");

        var stock = await _uow.Logistica.Maestros.StockArticulos
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.PlantaCode == planta && s.ArticuloCode == articuloCode,
                ct);

        var reservado = await AjusteInventarioRules.GetReservedAsync(_uow, planta, articuloCode, ct);

        return new AjusteInventarioArticuloInfoResponse
        {
            ArticuloCode = articulo.Code,
            ArticuloName = articulo.Name,
            UnidadMedidaCode = articulo.UnidadMedidaCode,
            Stock = stock?.Quantity ?? 0m,
            ReservadoPorAjustes = reservado,
            Precio = stock?.AverageCost ?? 0m
        };
    }
}
