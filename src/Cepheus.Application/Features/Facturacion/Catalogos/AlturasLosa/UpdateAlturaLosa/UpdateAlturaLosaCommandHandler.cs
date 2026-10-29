using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.UpdateAlturaLosa
{
    public class UpdateAlturaLosaCommandHandler : IRequestHandler<UpdateAlturaLosaCommand, AlturaLosaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAlturaLosaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AlturaLosaResponse> Handle(UpdateAlturaLosaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.AlturasLosa.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Altura de losa {request.Code} no encontrada.");
            }

            var alturaLosa = new AlturaLosa
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Value = request.Value,
                Width = request.Width,
                ProductoTipoCode = request.ProductoTipoCode,
                ProductoCode = request.ProductoCode,
                PolystyreneProductoTipoCode = request.PolystyreneProductoTipoCode,
                PolystyreneProductoCode = request.PolystyreneProductoCode,
                PolystyreneValue = request.PolystyreneValue,
                PolystyreneWidth = request.PolystyreneWidth,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.AlturasLosa.Update(alturaLosa);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La altura de losa fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new AlturaLosaResponse
            {
                Code = alturaLosa.Code,
                Name = alturaLosa.Name,
                Value = alturaLosa.Value,
                Width = alturaLosa.Width,
                ProductoTipoCode = alturaLosa.ProductoTipoCode,
                ProductoCode = alturaLosa.ProductoCode,
                PolystyreneProductoTipoCode = alturaLosa.PolystyreneProductoTipoCode,
                PolystyreneProductoCode = alturaLosa.PolystyreneProductoCode,
                PolystyreneValue = alturaLosa.PolystyreneValue,
                PolystyreneWidth = alturaLosa.PolystyreneWidth,
                IsActive = alturaLosa.IsActive,
                CreatedAt = alturaLosa.CreatedAt,
                UpdatedAt = alturaLosa.UpdatedAt,
                RowVersion = alturaLosa.RowVersion
            };
        }
    }
}
