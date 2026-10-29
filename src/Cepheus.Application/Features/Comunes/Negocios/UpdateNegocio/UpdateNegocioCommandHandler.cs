using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Negocios.Common;
using Cepheus.Domain.Comun;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Negocios.UpdateNegocio
{
    public class UpdateNegocioCommandHandler : IRequestHandler<UpdateNegocioCommand, NegocioResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNegocioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NegocioResponse> Handle(UpdateNegocioCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Comunes.Negocios.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Negocio {request.Code} no encontrado.");
            }

            var negocio = new Negocio
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Comunes.Negocios.Update(negocio);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El negocio fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new NegocioResponse
            {
                Code = negocio.Code,
                Name = negocio.Name,
                IsActive = negocio.IsActive,
                CreatedAt = negocio.CreatedAt,
                UpdatedAt = negocio.UpdatedAt,
                CreatedBy = negocio.CreatedBy,
                UpdatedBy = negocio.UpdatedBy,
                RowVersion = negocio.RowVersion
                
            };
        }
    }
}
