using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common;
using Cepheus.Domain.Contabilidad.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.CreateTipoAsiento
{
    public class CreateTipoAsientoCommandHandler
        : IRequestHandler<CreateTipoAsientoCommand, TipoAsientoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoAsientoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoAsientoResponse> Handle(
            CreateTipoAsientoCommand request,
            CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Contabilidad.Catalogos.TiposAsiento.Query().Select(t => t.Code),
                length: 2,
                entityLabel: "TiposAsiento",
                cancellationToken);

            var entity = new TipoAsiento
            {
                Code = code,
                Name = request.Name.Trim(),
                ExchangeRateType = request.ExchangeRateType,
                Valuation = request.Valuation,
                IsActive = true
            };

            await _uow.Contabilidad.Catalogos.TiposAsiento.AddAsync(
                entity,
                cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static TipoAsientoResponse Map(TipoAsiento e) => new()
        {
            Code = e.Code,
            Name = e.Name,
            ExchangeRateType = e.ExchangeRateType,
            Valuation = e.Valuation,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
