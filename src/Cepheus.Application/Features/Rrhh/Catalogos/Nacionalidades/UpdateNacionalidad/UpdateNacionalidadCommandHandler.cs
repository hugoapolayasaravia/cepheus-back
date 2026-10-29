using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.UpdateNacionalidad
{
    public class UpdateNacionalidadCommandHandler : IRequestHandler<UpdateNacionalidadCommand, NacionalidadResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNacionalidadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NacionalidadResponse> Handle(UpdateNacionalidadCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Nacionalidades.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Nacionalidad {request.Code} no encontrada.");
            }

            var nacionalidad = new Nacionalidad
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Nacionalidades.Update(nacionalidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La nacionalidad fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new NacionalidadResponse
            {
                Code = nacionalidad.Code,
                Name = nacionalidad.Name,
                IsActive = nacionalidad.IsActive,
                CreatedAt = nacionalidad.CreatedAt,
                UpdatedAt = nacionalidad.UpdatedAt,
                RowVersion = nacionalidad.RowVersion
            };
        }
    }
}