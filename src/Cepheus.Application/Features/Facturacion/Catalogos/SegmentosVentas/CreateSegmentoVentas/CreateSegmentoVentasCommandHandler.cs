using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.CreateSegmentoVentas
{
    public class CreateSegmentoVentasCommandHandler : IRequestHandler<CreateSegmentoVentasCommand, SegmentoVentasResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateSegmentoVentasCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SegmentoVentasResponse> Handle(CreateSegmentoVentasCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.SegmentosVentas.Query().Select(t => t.Code), length: 2, entityLabel: "Segmentos de Ventas", cancellationToken);

            var segmentoVentas = new SegmentoVentas
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.SegmentosVentas.AddAsync(segmentoVentas, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(segmentoVentas);
        }

        internal static SegmentoVentasResponse Map(SegmentoVentas segmentoVentas) => new()
        {
            Code = segmentoVentas.Code,
            Name = segmentoVentas.Name,
            IsActive = segmentoVentas.IsActive,
            CreatedAt = segmentoVentas.CreatedAt,
            UpdatedAt = segmentoVentas.UpdatedAt,
            RowVersion = segmentoVentas.RowVersion
        };
    }
}
