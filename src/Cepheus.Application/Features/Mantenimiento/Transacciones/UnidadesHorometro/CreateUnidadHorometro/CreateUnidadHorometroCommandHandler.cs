using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.CreateUnidadHorometro
{
    public class CreateUnidadHorometroCommandHandler : IRequestHandler<CreateUnidadHorometroCommand, UnidadHorometroResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateUnidadHorometroCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadHorometroResponse> Handle(CreateUnidadHorometroCommand request, CancellationToken cancellationToken)
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var fecha = request.Fecha.Date;

            var entity = new UnidadHorometro
            {
                SubCentroCostoCode = scc,
                Fecha = fecha,
                Horometro = request.Horometro,
                Estado = request.Estado,
                PlantaAfectadaCode = request.PlantaAfectadaCode.Trim().ToUpperInvariant(),
                EquipoCode = string.IsNullOrWhiteSpace(request.EquipoCode)
                    ? null
                    : request.EquipoCode.Trim().ToUpperInvariant()
            };

            await _uow.Mantenimiento.Transacciones.UnidadesHorometro.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return await _uow.Mantenimiento.Transacciones.UnidadesHorometro.Query()
                .AsNoTracking()
                .Where(x => x.SubCentroCostoCode == scc && x.Fecha == fecha)
                .Select(UnidadHorometroProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
