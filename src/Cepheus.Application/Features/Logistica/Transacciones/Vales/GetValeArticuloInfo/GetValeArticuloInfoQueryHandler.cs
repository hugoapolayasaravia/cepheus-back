using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValeArticuloInfo;

public sealed class GetValeArticuloInfoQueryHandler
    : IRequestHandler<GetValeArticuloInfoQuery, ValeArticuloInfoResponse>
{
    private readonly IUnitOfWork _uow;

    public GetValeArticuloInfoQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ValeArticuloInfoResponse> Handle(
        GetValeArticuloInfoQuery request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var articuloCode = ValeRules.Normalize(request.ArticuloCode);

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

        var reservado = await ValeRules.GetReservedAsync(_uow, planta, articuloCode, ct);
        var cantidadStock = stock?.Quantity ?? 0m;

        var response = new ValeArticuloInfoResponse
        {
            ArticuloCode = articulo.Code,
            ArticuloName = articulo.Name,
            UnidadMedidaCode = articulo.UnidadMedidaCode,
            Stock = cantidadStock,
            Reservado = reservado,
            Disponible = cantidadStock - reservado,
            Precio = stock?.AverageCost ?? 0m,
            RequiresHorometro = articulo.RequiresHorometro
        };

        var tipoVale = ValeRules.NormalizeOrNull(request.TipoValeCode);

        if (tipoVale is not null)
        {
            response.PermitidoParaTipoVale = await _uow.Logistica.Catalogos.TiposValeArticulo
                .Query()
                .AsNoTracking()
                .AnyAsync(
                    x => x.TipoValeCode == tipoVale && x.ArticuloCode == articuloCode,
                    ct);
        }

        // Logi_sp_Consulta_Horometros: último horómetro procesado del artículo para el mismo subcentro de
        // costo, en vales anteriores a la fecha indicada (el más reciente por código de vale).
        var subCentro = ValeRules.NormalizeOrNull(request.SubCentroCostoCode);

        if (articulo.RequiresHorometro && subCentro is not null)
        {
            var fecha = (request.FechaEntrega ?? ValeRules.GetProcessDate()).Date;

            response.HorometroAnterior = await _uow.Logistica.Transacciones.ValeDetalles
                .Query()
                .AsNoTracking()
                .Where(d => d.PlantaCode == planta &&
                            d.ArticuloCode == articuloCode &&
                            d.Estado == EstadoValeDetalle.Procesado &&
                            d.Vale.SubCentroCostoCode == subCentro &&
                            d.Vale.FechaEntrega < fecha)
                .OrderByDescending(d => d.ValeCode)
                .Select(d => d.Propiedad01)
                .FirstOrDefaultAsync(ct);
        }

        return response;
    }
}
