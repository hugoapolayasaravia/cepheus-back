using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Epss.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Epss.UpdateEps
{
    public class UpdateEpsCommandHandler : IRequestHandler<UpdateEpsCommand, EpsResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEpsCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EpsResponse> Handle(UpdateEpsCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Epss.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"EPS {request.Code} no encontrada.");
            }

            var eps = new Eps
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Epss.Update(eps);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La EPS fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new EpsResponse
            {
                Code = eps.Code,
                Name = eps.Name,
                IsActive = eps.IsActive,
                CreatedAt = eps.CreatedAt,
                UpdatedAt = eps.UpdatedAt,
                RowVersion = eps.RowVersion
            };
        }
    }
}