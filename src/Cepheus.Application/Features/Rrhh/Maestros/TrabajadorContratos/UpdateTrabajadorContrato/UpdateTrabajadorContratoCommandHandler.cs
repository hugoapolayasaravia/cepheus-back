using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.UpdateTrabajadorContrato
{
    public class UpdateTrabajadorContratoCommandHandler : IRequestHandler<UpdateTrabajadorContratoCommand, TrabajadorContratoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorContratoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorContratoResponse> Handle(UpdateTrabajadorContratoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorContratos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorContrato {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorContrato
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                TipoContratoCode = request.TipoContratoCode,
                TipoExtensionCode = request.TipoExtensionCode,
                FechaInicio = request.FechaInicio,
                FechaFin = request.FechaFin,
                FechaTermino = request.FechaTermino,
                Renovado = request.Renovado,
                TipoDuracion = string.IsNullOrWhiteSpace(request.TipoDuracion) ? null : request.TipoDuracion!.Trim(),
                CantidadDuracion = request.CantidadDuracion,
                Activo = request.Activo,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorContratos.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorContrato.CreateTrabajadorContratoCommandHandler.Map(entidad);
        }
    }
}
