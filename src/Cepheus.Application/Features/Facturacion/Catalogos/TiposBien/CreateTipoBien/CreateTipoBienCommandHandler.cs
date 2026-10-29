using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.CreateTipoBien
{
    public class CreateTipoBienCommandHandler
        : IRequestHandler<CreateTipoBienCommand, TipoBienResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoBienCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoBienResponse> Handle(
            CreateTipoBienCommand request,
            CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.TiposBien.Query().Select(t => t.Code),
                length: 3,
                entityLabel: "TiposBien",
                cancellationToken);

            var tipoBien = new TipoBien
            {
                Code = code,
                Name = request.Name.Trim(),
                DetractionRate = request.DetractionRate,
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.TiposBien.AddAsync(
                tipoBien,
                cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoBien);
        }

        internal static TipoBienResponse Map(TipoBien e) => new()
        {
            Code = e.Code,
            Name = e.Name,
            DetractionRate = e.DetractionRate,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}