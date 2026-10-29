using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.UpdateTrabajadorSalud
{
    public class UpdateTrabajadorSaludCommandHandler : IRequestHandler<UpdateTrabajadorSaludCommand, TrabajadorSaludResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorSaludCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorSaludResponse> Handle(UpdateTrabajadorSaludCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorSaluds.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorSalud {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorSalud
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                TipoSangreCode = request.TipoSangreCode,
                AlergiaCode = request.AlergiaCode,
                Otros = string.IsNullOrWhiteSpace(request.Otros) ? null : request.Otros!.Trim(),
                FechaEvaluacionMedica = request.FechaEvaluacionMedica,
                Observaciones = string.IsNullOrWhiteSpace(request.Observaciones) ? null : request.Observaciones!.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorSaluds.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorSalud.CreateTrabajadorSaludCommandHandler.Map(entidad);
        }
    }
}
