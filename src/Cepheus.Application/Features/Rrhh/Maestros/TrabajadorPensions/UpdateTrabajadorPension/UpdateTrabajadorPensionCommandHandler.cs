using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.UpdateTrabajadorPension
{
    public class UpdateTrabajadorPensionCommandHandler : IRequestHandler<UpdateTrabajadorPensionCommand, TrabajadorPensionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorPensionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorPensionResponse> Handle(UpdateTrabajadorPensionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorPensions.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorPension {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorPension
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                TipoAfiliacionCode = request.TipoAfiliacionCode,
                AfpCode = request.AfpCode,
                FechaAfiliacion = request.FechaAfiliacion,
                NumeroAfp = string.IsNullOrWhiteSpace(request.NumeroAfp) ? null : request.NumeroAfp!.Trim(),
                RegimenPensionarioCode = request.RegimenPensionarioCode,
                TipoPensionCode = request.TipoPensionCode,
                NumeroCarnetSsp = string.IsNullOrWhiteSpace(request.NumeroCarnetSsp) ? null : request.NumeroCarnetSsp!.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorPensions.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorPension.CreateTrabajadorPensionCommandHandler.Map(entidad);
        }
    }
}
