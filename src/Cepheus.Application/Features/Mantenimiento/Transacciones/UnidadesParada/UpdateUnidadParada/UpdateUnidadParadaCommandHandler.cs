using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.UpdateUnidadParada
{
    public class UpdateUnidadParadaCommandHandler : IRequestHandler<UpdateUnidadParadaCommand, UnidadParadaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUnidadParadaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadParadaResponse> Handle(UpdateUnidadParadaCommand request, CancellationToken cancellationToken)
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var fecha = request.Fecha.Date;

            var current = await _uow.Mantenimiento.Transacciones.UnidadesParada.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SubCentroCostoCode == scc && x.Fecha == fecha, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Registro {scc}/{fecha:yyyy-MM-dd} no encontrado.");
            }

            var entity = new UnidadParada
            {
                SubCentroCostoCode = scc,
                Fecha = fecha,

                Horas = request.Horas,
                Estado = request.Estado,
                PlantaAfectadaCode = request.PlantaAfectadaCode.Trim().ToUpperInvariant(),
                FallaCode = string.IsNullOrWhiteSpace(request.FallaCode)
                    ? null
                    : request.FallaCode.Trim().ToUpperInvariant(),

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Transacciones.UnidadesParada.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return await _uow.Mantenimiento.Transacciones.UnidadesParada.Query()
                .AsNoTracking()
                .Where(x => x.SubCentroCostoCode == scc && x.Fecha == fecha)
                .Select(UnidadParadaProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
