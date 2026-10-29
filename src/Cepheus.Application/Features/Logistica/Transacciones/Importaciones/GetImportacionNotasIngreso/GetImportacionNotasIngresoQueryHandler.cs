// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GetImportacionNotasIngreso/GetImportacionNotasIngresoQueryHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionNotasIngreso
{
    public class GetImportacionNotasIngresoQueryHandler
        : IRequestHandler<GetImportacionNotasIngresoQuery, List<NotaIngresoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetImportacionNotasIngresoQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<NotaIngresoResponse>> Handle(GetImportacionNotasIngresoQuery request, CancellationToken ct)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.ImportacionCode.Trim().ToUpperInvariant();

            var existe = await _uow.Logistica.Transacciones.Importaciones.Query()
                .AsNoTracking().AnyAsync(i => i.PlantaCode == planta && i.Code == code, ct);
            if (!existe)
                throw new KeyNotFoundException($"Importación {planta}/{code} no encontrada.");

            var notas = await NotaIngresoReader.WithIncludes(_uow.Logistica.Transacciones.NotaIngresos.Query())
                .Where(n => n.PlantaCode == planta && n.ImportacionCode == code && n.Origen == OrigenNotaIngreso.Importacion)
                .OrderByDescending(n => n.FechaProceso)
                .ToListAsync(ct);

            return notas.Select(NotaIngresoMapper.Map).ToList();
        }
    }
}
