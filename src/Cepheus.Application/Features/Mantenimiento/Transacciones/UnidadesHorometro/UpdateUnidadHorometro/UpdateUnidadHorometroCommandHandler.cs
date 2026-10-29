using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.UpdateUnidadHorometro
{
    public class UpdateUnidadHorometroCommandHandler : IRequestHandler<UpdateUnidadHorometroCommand, UnidadHorometroResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUnidadHorometroCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadHorometroResponse> Handle(UpdateUnidadHorometroCommand request, CancellationToken cancellationToken)
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var fecha = request.Fecha.Date;

            var current = await _uow.Mantenimiento.Transacciones.UnidadesHorometro.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SubCentroCostoCode == scc && x.Fecha == fecha, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Registro {scc}/{fecha:yyyy-MM-dd} no encontrado.");
            }

            var entity = new UnidadHorometro
            {
                SubCentroCostoCode = scc,
                Fecha = fecha,

                Horometro = request.Horometro,
                Estado = request.Estado,
                PlantaAfectadaCode = request.PlantaAfectadaCode.Trim().ToUpperInvariant(),
                EquipoCode = string.IsNullOrWhiteSpace(request.EquipoCode)
                    ? null
                    : request.EquipoCode.Trim().ToUpperInvariant(),

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Transacciones.UnidadesHorometro.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return await _uow.Mantenimiento.Transacciones.UnidadesHorometro.Query()
                .AsNoTracking()
                .Where(x => x.SubCentroCostoCode == scc && x.Fecha == fecha)
                .Select(UnidadHorometroProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
