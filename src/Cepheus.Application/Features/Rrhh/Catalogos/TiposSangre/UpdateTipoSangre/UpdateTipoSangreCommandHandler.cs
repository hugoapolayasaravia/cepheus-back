using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.UpdateTipoSangre
{
    public class UpdateTipoSangreCommandHandler : IRequestHandler<UpdateTipoSangreCommand, TipoSangreResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoSangreCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoSangreResponse> Handle(UpdateTipoSangreCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposSangre.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de sangre {request.Code} no encontrado.");
            }

            var tipoSangre = new TipoSangre
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposSangre.Update(tipoSangre);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de sangre fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoSangreResponse
            {
                Code = tipoSangre.Code,
                Name = tipoSangre.Name,
                IsActive = tipoSangre.IsActive,
                CreatedAt = tipoSangre.CreatedAt,
                UpdatedAt = tipoSangre.UpdatedAt,
                RowVersion = tipoSangre.RowVersion
            };
        }
    }
}