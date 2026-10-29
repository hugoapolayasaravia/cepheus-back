using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.UpdateSituacionEps
{
    public class UpdateSituacionEpsCommandHandler : IRequestHandler<UpdateSituacionEpsCommand, SituacionEpsResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSituacionEpsCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SituacionEpsResponse> Handle(UpdateSituacionEpsCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.SituacionesEps.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Situación EPS {request.Code} no encontrada.");
            }

            var situacionEps = new SituacionEps
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.SituacionesEps.Update(situacionEps);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La situación EPS fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new SituacionEpsResponse
            {
                Code = situacionEps.Code,
                Name = situacionEps.Name,
                IsActive = situacionEps.IsActive,
                CreatedAt = situacionEps.CreatedAt,
                UpdatedAt = situacionEps.UpdatedAt,
                RowVersion = situacionEps.RowVersion
            };
        }
    }
}