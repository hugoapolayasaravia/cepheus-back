using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.UpdateTipoArticulo
{
    public class UpdateTipoArticuloCommandHandler : IRequestHandler<UpdateTipoArticuloCommand, TipoArticuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoArticuloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoArticuloResponse> Handle(UpdateTipoArticuloCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.TiposArticulo.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de artículo {request.Code} no encontrado.");
            }

            var tipoArticulo = new TipoArticulo
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Description = request.Description,
                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.TiposArticulo.Update(tipoArticulo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de artículo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoArticuloResponse
            {
                Code = tipoArticulo.Code,
                Name = tipoArticulo.Name,
                Description = tipoArticulo.Description,
                IsActive = tipoArticulo.IsActive,
                CreatedAt = tipoArticulo.CreatedAt,
                UpdatedAt = tipoArticulo.UpdatedAt,
                CreatedBy = tipoArticulo.CreatedBy,
                UpdatedBy = tipoArticulo.UpdatedBy,
                RowVersion = tipoArticulo.RowVersion
            };
        }
    }
}