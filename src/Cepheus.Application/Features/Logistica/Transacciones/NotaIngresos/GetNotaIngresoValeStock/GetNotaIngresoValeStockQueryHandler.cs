using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresoValeStock;

public sealed class GetNotaIngresoValeStockQueryHandler
    : IRequestHandler<GetNotaIngresoValeStockQuery, IReadOnlyList<LineaSinStock>>
{
    private readonly IUnitOfWork _uow;

    public GetNotaIngresoValeStockQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<LineaSinStock>> Handle(GetNotaIngresoValeStockQuery request, CancellationToken ct)
    {
        var planta = NotaIngresoRules.Normalize(request.PlantaCode);
        var code = NotaIngresoRules.Normalize(request.NotaIngresoCode);

        var note = await _uow.Logistica.Transacciones.NotaIngresos.Query().AsNoTracking()
            .Include(n => n.Detalles)
            .FirstOrDefaultAsync(n => n.PlantaCode == planta && n.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Nota de Ingreso {planta}/{code} no existe.");

        var lineas = note.Detalles.Where(d => d.Estado == EstadoNotaIngresoDetalle.Procesado).ToList();

        return await NotaIngresoValeRules.GetLineasSinStockAsync(_uow, planta, lineas, ct);
    }
}
