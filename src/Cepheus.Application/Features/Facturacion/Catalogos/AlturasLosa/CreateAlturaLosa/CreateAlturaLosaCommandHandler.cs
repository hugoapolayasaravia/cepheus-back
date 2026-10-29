using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.CreateAlturaLosa
{
    public class CreateAlturaLosaCommandHandler : IRequestHandler<CreateAlturaLosaCommand, AlturaLosaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateAlturaLosaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AlturaLosaResponse> Handle(CreateAlturaLosaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.AlturasLosa.Query().Select(t => t.Code), length: 2, entityLabel: "Alturas de Losa", cancellationToken);

            var alturaLosa = new AlturaLosa
            {
                Code = code,
                Name = request.Name.Trim(),
                Value = request.Value,
                Width = request.Width,
                ProductoTipoCode = request.ProductoTipoCode,
                ProductoCode = request.ProductoCode,
                PolystyreneProductoTipoCode = request.PolystyreneProductoTipoCode,
                PolystyreneProductoCode = request.PolystyreneProductoCode,
                PolystyreneValue = request.PolystyreneValue,
                PolystyreneWidth = request.PolystyreneWidth,
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.AlturasLosa.AddAsync(alturaLosa, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(alturaLosa);
        }

        internal static AlturaLosaResponse Map(AlturaLosa alturaLosa) => new()
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
