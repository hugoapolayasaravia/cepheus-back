using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Afps.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Afps.UpdateAfp
{
    public class UpdateAfpCommandHandler : IRequestHandler<UpdateAfpCommand, AfpResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAfpCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AfpResponse> Handle(UpdateAfpCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Afps.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"AFP {request.Code} no encontrada.");
            }

            var afp = new Afp
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Afps.Update(afp);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La AFP fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new AfpResponse
            {
                Code = afp.Code,
                Name = afp.Name,
                IsActive = afp.IsActive,
                CreatedAt = afp.CreatedAt,
                UpdatedAt = afp.UpdatedAt,
                RowVersion = afp.RowVersion
            };
        }
    }
}