using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.UpdateObjetoActividad
{
    public class UpdateObjetoActividadCommandHandler : IRequestHandler<UpdateObjetoActividadCommand, ObjetoActividadResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateObjetoActividadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ObjetoActividadResponse> Handle(UpdateObjetoActividadCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Maestros.ObjetosActividad.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Objeto de actividad {request.Code} no encontrado.");
            }

            var objetoActividad = new ObjetoActividad
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Maestros.ObjetosActividad.Update(objetoActividad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El objeto de actividad fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ObjetoActividadResponse
            {
                Code = objetoActividad.Code,
                Name = objetoActividad.Name,
                IsActive = objetoActividad.IsActive,
                CreatedAt = objetoActividad.CreatedAt,
                UpdatedAt = objetoActividad.UpdatedAt,
                RowVersion = objetoActividad.RowVersion
            };
        }
    }
}
