using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.UpdateEspecialidad
{
    public class UpdateEspecialidadTrabajadorCommandHandler : IRequestHandler<UpdateEspecialidadTrabajadorCommand, EspecialidadTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEspecialidadTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EspecialidadTrabajadorResponse> Handle(UpdateEspecialidadTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.EspecialidadesTrabajador.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Especialidad {request.Code} no encontrada.");
            }

            var especialidad = new EspecialidadTrabajador
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.EspecialidadesTrabajador.Update(especialidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La especialidad fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new EspecialidadTrabajadorResponse
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