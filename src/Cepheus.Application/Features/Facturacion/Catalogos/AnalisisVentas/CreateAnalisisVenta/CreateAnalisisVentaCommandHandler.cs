using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.CreateAnalisisVenta
{
    public class CreateAnalisisVentaCommandHandler : IRequestHandler<CreateAnalisisVentaCommand, AnalisisVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateAnalisisVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AnalisisVentaResponse> Handle(CreateAnalisisVentaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.AnalisisVentas.Query().Select(a => a.Code), length: 3, entityLabel: "Análisis de Ventas", cancellationToken);

            var analisisVenta = new AnalisisVenta
            {
                Code = code,
                Name = request.Name.Trim(),
                ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim(),
                SegmentoVentasCode = string.IsNullOrWhiteSpace(request.SegmentoVentasCode) ? null : request.SegmentoVentasCode.Trim().ToUpperInvariant(),
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.AnalisisVentas.AddAsync(analisisVenta, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(analisisVenta);
        }

        internal static AnalisisVentaResponse Map(AnalisisVenta analisisVenta) => new()
        {
            Code = analisisVenta.Code,
            Name = analisisVenta.Name,
            ShortName = analisisVenta.ShortName,
            SegmentoVentasCode = analisisVenta.SegmentoVentasCode,
            IsActive = analisisVenta.IsActive,
            CreatedAt = analisisVenta.CreatedAt,
            UpdatedAt = analisisVenta.UpdatedAt,
            RowVersion = analisisVenta.RowVersion
        };
    }
}
