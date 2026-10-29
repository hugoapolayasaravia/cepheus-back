using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Tramites.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.UpdateTramite
{
    public class UpdateTramiteCommandHandler : IRequestHandler<UpdateTramiteCommand, TramiteResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTramiteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TramiteResponse> Handle(UpdateTramiteCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.Tramites.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Trámite {request.Code} no encontrado.");
            }

            var tramite = new Tramite
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.Tramites.Update(tramite);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El trámite fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TramiteResponse
            {
                Code = tramite.Code,
                Name = tramite.Name,
                IsActive = tramite.IsActive,
                CreatedAt = tramite.CreatedAt,
                UpdatedAt = tramite.UpdatedAt,
                CreatedBy = tramite.CreatedBy,
                UpdatedBy = tramite.UpdatedBy,
                RowVersion = tramite.RowVersion
            };
        }
    }
}