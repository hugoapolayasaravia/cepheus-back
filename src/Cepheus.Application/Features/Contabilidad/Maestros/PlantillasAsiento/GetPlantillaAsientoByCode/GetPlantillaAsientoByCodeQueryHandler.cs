using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.GetPlantillaAsientoByCode
{
    public class GetPlantillaAsientoByCodeQueryHandler
        : IRequestHandler<GetPlantillaAsientoByCodeQuery, PlantillaAsientoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetPlantillaAsientoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlantillaAsientoResponse> Handle(
            GetPlantillaAsientoByCodeQuery request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var entity = await _uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .AsNoTracking()
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Plantilla de asiento {code} no encontrada.");
            }

            return PlantillaAsientoMapper.Map(entity);
        }
    }
}
