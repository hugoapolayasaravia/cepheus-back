using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.UpdateTipoValorizacion
{
    public class UpdateTipoValorizacionCommandHandler : IRequestHandler<UpdateTipoValorizacionCommand, TipoValorizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoValorizacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoValorizacionResponse> Handle(UpdateTipoValorizacionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.TiposValorizacion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de valorización {request.Code} no encontrado.");
            }

            var tipoValorizacion = new TipoValorizacion
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Days = request.Days,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.TiposValorizacion.Update(tipoValorizacion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de valorización fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoValorizacionResponse
            {
                Code = tipoValorizacion.Code,
                Name = tipoValorizacion.Name,
                Days = tipoValorizacion.Days,
                IsActive = tipoValorizacion.IsActive,
                CreatedAt = tipoValorizacion.CreatedAt,
                UpdatedAt = tipoValorizacion.UpdatedAt,
                RowVersion = tipoValorizacion.RowVersion
            };
        }
    }
}
