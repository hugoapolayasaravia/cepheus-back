using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.Common;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.UpdateNoAfectoRetencion
{
    public class UpdateNoAfectoRetencionCommandHandler : IRequestHandler<UpdateNoAfectoRetencionCommand, NoAfectoRetencionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNoAfectoRetencionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NoAfectoRetencionResponse> Handle(UpdateNoAfectoRetencionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Contabilidad.Maestros.NoAfectosRetencion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Ruc == request.Ruc && x.Type == request.Type, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"No afecto a retención {request.Ruc}/{request.Type} no encontrado.");
            }

            var entity = new NoAfectoRetencion
            {
                Ruc = request.Ruc,
                Type = request.Type,
                Name = request.Name.Trim(),
                Resolution = request.Resolution.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Contabilidad.Maestros.NoAfectosRetencion.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateNoAfectoRetencion.CreateNoAfectoRetencionCommandHandler.Map(entity);
        }
    }
}
