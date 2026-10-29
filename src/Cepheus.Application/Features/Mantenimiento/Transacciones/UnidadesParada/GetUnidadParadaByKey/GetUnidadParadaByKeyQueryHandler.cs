using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.GetUnidadParadaByKey
{
    public class GetUnidadParadaByKeyQueryHandler : IRequestHandler<GetUnidadParadaByKeyQuery, UnidadParadaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetUnidadParadaByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadParadaResponse> Handle(GetUnidadParadaByKeyQuery request, CancellationToken cancellationToken)
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var fecha = request.Fecha.Date;

            var result = await _uow.Mantenimiento.Transacciones.UnidadesParada.Query()
                .AsNoTracking()
                .Where(x => x.SubCentroCostoCode == scc && x.Fecha == fecha)
                .Select(UnidadParadaProjection.ToResponse)
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
            {
                throw new KeyNotFoundException($"Registro {scc}/{fecha:yyyy-MM-dd} no encontrado.");
            }

            return result;
        }
    }
}
