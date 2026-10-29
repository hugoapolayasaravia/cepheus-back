using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresoReferencia;

public sealed class GetNotaIngresoReferenciaQueryHandler
    : IRequestHandler<GetNotaIngresoReferenciaQuery, NotaIngresoResponse>
{
    private readonly IUnitOfWork _uow;

    public GetNotaIngresoReferenciaQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<NotaIngresoResponse> Handle(GetNotaIngresoReferenciaQuery request, CancellationToken ct)
    {
        var planta = NotaIngresoRules.Normalize(request.PlantaCode);
        var proveedor = NotaIngresoRules.Normalize(request.ProveedorCode);
        var numero = request.NumeroDocumento.Trim();

        var note = await NotaIngresoReader.WithIncludes(_uow.Logistica.Transacciones.NotaIngresos.Query())
            .Where(x => x.PlantaCode == planta
                        && x.ComprobantePagoCode == request.ComprobantePagoReferenciaCode
                        && x.ProveedorCode == proveedor
                        && x.NumeroDocumento == numero
                        && x.Estado != EstadoNotaIngreso.Anulado)
            .OrderByDescending(x => x.FechaProceso)
            .FirstOrDefaultAsync(ct);

        if (note is null)
            throw new KeyNotFoundException("No se encontró una Nota de Ingreso vigente para el documento indicado.");

        return NotaIngresoMapper.Map(note);
    }
}
