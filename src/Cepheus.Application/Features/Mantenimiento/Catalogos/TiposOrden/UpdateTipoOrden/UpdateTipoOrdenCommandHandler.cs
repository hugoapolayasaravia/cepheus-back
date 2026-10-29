using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.UpdateTipoOrden
{
    public class UpdateTipoOrdenCommandHandler : IRequestHandler<UpdateTipoOrdenCommand, TipoOrdenResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoOrdenCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoOrdenResponse> Handle(UpdateTipoOrdenCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Catalogos.TiposOrden.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de orden {request.Code} no encontrado.");
            }

            var tipoOrden = new TipoOrden
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Catalogos.TiposOrden.Update(tipoOrden);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de orden fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoOrdenResponse
            {
                Code = tipoOrden.Code,
                Name = tipoOrden.Name,
                IsActive = tipoOrden.IsActive,
                CreatedAt = tipoOrden.CreatedAt,
                UpdatedAt = tipoOrden.UpdatedAt,
                RowVersion = tipoOrden.RowVersion
            };
        }
    }
}
