using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.UpdateTipoOperacion
{
    public class UpdateTipoOperacionCommandHandler : IRequestHandler<UpdateTipoOperacionCommand, TipoOperacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoOperacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoOperacionResponse> Handle(UpdateTipoOperacionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.TiposOperacion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de operación {request.Code} no encontrado.");
            }

            var entity = new TipoOperacion
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.TiposOperacion.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTipoOperacion.CreateTipoOperacionCommandHandler.Map(entity);
        }
    }
}
