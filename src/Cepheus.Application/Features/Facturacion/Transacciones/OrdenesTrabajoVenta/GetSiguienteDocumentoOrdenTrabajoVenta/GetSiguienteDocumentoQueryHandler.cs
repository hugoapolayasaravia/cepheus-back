using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetSiguienteDocumentoOrdenTrabajoVenta;

public sealed class GetSiguienteDocumentoQueryHandler : IRequestHandler<GetSiguienteDocumentoQuery, SiguienteDocumentoResponse>
{
    private readonly IUnitOfWork _uow;

    public GetSiguienteDocumentoQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<SiguienteDocumentoResponse> Handle(GetSiguienteDocumentoQuery request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeFixed(request.Planta, 2);

        var planta = await _uow.Comunes.Plantas
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(p => p.Code == code, ct)
            ?? throw new KeyNotFoundException($"La planta {code} no existe.");

        var last = OrdenTrabajoVentaDocumentos.GetLast(planta, request.Tipo);

        return new SiguienteDocumentoResponse
        {
            PlantCode = planta.Code,
            PlantName = planta.Name,
            DocumentType = request.Tipo,
            LastNumber = last,
            NextNumber = OrdenTrabajoVentaDocumentos.Next(last)
        };
    }
}
