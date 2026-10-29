using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.UpdateTrabajadorSindicato
{
    public class UpdateTrabajadorSindicatoCommandHandler : IRequestHandler<UpdateTrabajadorSindicatoCommand, TrabajadorSindicatoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorSindicatoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorSindicatoResponse> Handle(UpdateTrabajadorSindicatoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorSindicatos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorSindicato {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorSindicato
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                Afiliado = request.Afiliado,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorSindicatos.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorSindicato.CreateTrabajadorSindicatoCommandHandler.Map(entidad);
        }
    }
}
