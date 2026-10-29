using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.GetUnidadHorometroByKey
{
    public class GetUnidadHorometroByKeyQueryHandler : IRequestHandler<GetUnidadHorometroByKeyQuery, UnidadHorometroResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetUnidadHorometroByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadHorometroResponse> Handle(GetUnidadHorometroByKeyQuery request, CancellationToken cancellationToken)
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var fecha = request.Fecha.Date;

            var result = await _uow.Mantenimiento.Transacciones.UnidadesHorometro.Query()
                .AsNoTracking()
                .Where(x => x.SubCentroCostoCode == scc && x.Fecha == fecha)
                .Select(UnidadHorometroProjection.ToResponse)
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
            {
                throw new KeyNotFoundException($"Registro {scc}/{fecha:yyyy-MM-dd} no encontrado.");
            }

            return result;
        }
    }
}
