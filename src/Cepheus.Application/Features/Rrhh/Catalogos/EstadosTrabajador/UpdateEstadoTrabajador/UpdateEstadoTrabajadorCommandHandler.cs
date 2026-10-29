using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.UpdateEstadoTrabajador
{
    public class UpdateEstadoTrabajadorCommandHandler : IRequestHandler<UpdateEstadoTrabajadorCommand, EstadoTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEstadoTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EstadoTrabajadorResponse> Handle(UpdateEstadoTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.EstadosTrabajador.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Estado de trabajador {request.Code} no encontrado.");
            }

            var estadoTrabajador = new EstadoTrabajador
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.EstadosTrabajador.Update(estadoTrabajador);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El estado de trabajador fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new EstadoTrabajadorResponse
            {
                Code = estadoTrabajador.Code,
                Name = estadoTrabajador.Name,
                IsActive = estadoTrabajador.IsActive,
                CreatedAt = estadoTrabajador.CreatedAt,
                UpdatedAt = estadoTrabajador.UpdatedAt,
                RowVersion = estadoTrabajador.RowVersion
            };
        }
    }
}