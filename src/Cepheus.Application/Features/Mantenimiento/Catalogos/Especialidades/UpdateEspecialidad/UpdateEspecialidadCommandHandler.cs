using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.UpdateEspecialidad
{
    public class UpdateEspecialidadCommandHandler : IRequestHandler<UpdateEspecialidadCommand, EspecialidadResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEspecialidadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EspecialidadResponse> Handle(UpdateEspecialidadCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Catalogos.Especialidades.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Especialidad {request.Code} no encontrada.");
            }

            var especialidad = new Especialidad
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Catalogos.Especialidades.Update(especialidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La especialidad fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new EspecialidadResponse
            {
                Code = especialidad.Code,
                Name = especialidad.Name,
                IsActive = especialidad.IsActive,
                CreatedAt = especialidad.CreatedAt,
                UpdatedAt = especialidad.UpdatedAt,
                RowVersion = especialidad.RowVersion
            };
        }
    }
}
