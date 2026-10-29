using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.UpdateTrabajadorRemuneracion
{
    public class UpdateTrabajadorRemuneracionCommandHandler : IRequestHandler<UpdateTrabajadorRemuneracionCommand, TrabajadorRemuneracionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorRemuneracionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorRemuneracionResponse> Handle(UpdateTrabajadorRemuneracionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorRemuneracions.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorRemuneracion {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorRemuneracion
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                SueldoBasico = request.SueldoBasico,
                MonedaCode = string.IsNullOrWhiteSpace(request.MonedaCode) ? null : request.MonedaCode!.Trim(),
                ModoPagoCode = request.ModoPagoCode,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorRemuneracions.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorRemuneracion.CreateTrabajadorRemuneracionCommandHandler.Map(entidad);
        }
    }
}
