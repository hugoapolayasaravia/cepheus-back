using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.GetAlturaLosaByCode
{
    public class GetAlturaLosaByCodeQueryHandler : IRequestHandler<GetAlturaLosaByCodeQuery, AlturaLosaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetAlturaLosaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AlturaLosaResponse> Handle(GetAlturaLosaByCodeQuery request, CancellationToken cancellationToken)
        {
            var alturaLosa = await _uow.Facturacion.Catalogos.AlturasLosa.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new AlturaLosaResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    Value = t.Value,
                    Width = t.Width,
                    ProductoTipoCode = t.ProductoTipoCode,
                    ProductoCode = t.ProductoCode,
                    PolystyreneProductoTipoCode = t.PolystyreneProductoTipoCode,
                    PolystyreneProductoCode = t.PolystyreneProductoCode,
                    PolystyreneValue = t.PolystyreneValue,
                    PolystyreneWidth = t.PolystyreneWidth,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (alturaLosa is null)
            {
                throw new KeyNotFoundException($"Altura de losa {request.Code} no encontrada.");
            }

            return alturaLosa;
        }
    }
}
