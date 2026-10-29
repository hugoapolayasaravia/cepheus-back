using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.UpdateTrabajadorFormacion
{
    public class UpdateTrabajadorFormacionCommandHandler : IRequestHandler<UpdateTrabajadorFormacionCommand, TrabajadorFormacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorFormacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorFormacionResponse> Handle(UpdateTrabajadorFormacionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorFormacions.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorFormacion {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorFormacion
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                NivelEducativoCode = request.NivelEducativoCode,
                GradoInstruccionCode = request.GradoInstruccionCode,
                TituloCode = request.TituloCode,
                EspecialidadCode = request.EspecialidadCode,
                TipoCentroFormacionCode = request.TipoCentroFormacionCode,
                ModalidadFormativaCode = request.ModalidadFormativaCode,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorFormacions.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorFormacion.CreateTrabajadorFormacionCommandHandler.Map(entidad);
        }
    }
}
