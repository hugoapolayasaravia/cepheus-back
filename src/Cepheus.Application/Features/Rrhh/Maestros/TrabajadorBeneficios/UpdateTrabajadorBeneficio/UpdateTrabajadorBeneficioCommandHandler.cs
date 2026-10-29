using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.UpdateTrabajadorBeneficio
{
    public class UpdateTrabajadorBeneficioCommandHandler : IRequestHandler<UpdateTrabajadorBeneficioCommand, TrabajadorBeneficioResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorBeneficioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorBeneficioResponse> Handle(UpdateTrabajadorBeneficioCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorBeneficios.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorBeneficio {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorBeneficio
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                Cts = request.Cts,
                Gratificacion = request.Gratificacion,
                Vacaciones = request.Vacaciones,
                MovilidadAntesEntrada = request.MovilidadAntesEntrada,
                MovilidadDespuesSalida = request.MovilidadDespuesSalida,
                Refrigerio = request.Refrigerio,
                Cena = request.Cena,
                Vale = request.Vale,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorBeneficios.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorBeneficio.CreateTrabajadorBeneficioCommandHandler.Map(entidad);
        }
    }
}
