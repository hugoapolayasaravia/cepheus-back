using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.UpdateVerboActividad
{
    public class UpdateVerboActividadCommandHandler : IRequestHandler<UpdateVerboActividadCommand, VerboActividadResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateVerboActividadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<VerboActividadResponse> Handle(UpdateVerboActividadCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Maestros.VerbosActividad.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Verbo de actividad {request.Code} no encontrado.");
            }

            var verboActividad = new VerboActividad
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Maestros.VerbosActividad.Update(verboActividad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El verbo de actividad fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new VerboActividadResponse
            {
                Code = verboActividad.Code,
                Name = verboActividad.Name,
                IsActive = verboActividad.IsActive,
                CreatedAt = verboActividad.CreatedAt,
                UpdatedAt = verboActividad.UpdatedAt,
                RowVersion = verboActividad.RowVersion
            };
        }
    }
}
