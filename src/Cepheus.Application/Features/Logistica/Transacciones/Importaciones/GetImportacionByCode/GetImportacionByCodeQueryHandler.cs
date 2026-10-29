// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GetImportacionByCode/GetImportacionByCodeQueryHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionByCode
{
    public class GetImportacionByCodeQueryHandler : IRequestHandler<GetImportacionByCodeQuery, ImportacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetImportacionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportacionResponse> Handle(GetImportacionByCodeQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var importacion = await _uow.Logistica.Transacciones.Importaciones.Query()
                .AsNoTracking()
                .Include(i => i.Detalles)
                .Include(i => i.Gastos).ThenInclude(g => g.Articulos)
                .AsSplitQuery()
                .FirstOrDefaultAsync(i => i.PlantaCode == plantaCode && i.Code == code, cancellationToken);

            if (importacion is null)
            {
                throw new KeyNotFoundException($"Importación {plantaCode}/{code} no encontrada.");
            }

            return ImportacionMapper.Map(importacion);
        }
    }
}
