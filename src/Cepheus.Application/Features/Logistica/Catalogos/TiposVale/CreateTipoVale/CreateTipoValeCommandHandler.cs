using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposVale.CreateTipoVale
{
    public class CreateTipoValeCommandHandler : IRequestHandler<CreateTipoValeCommand, TipoValeResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoValeCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoValeResponse> Handle(CreateTipoValeCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.TiposVale.Query().Select(t => t.Code), length: 3, entityLabel: "Tipos de Vale", cancellationToken);

            var tipoVale = new TipoVale
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.TiposVale.AddAsync(tipoVale, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoVale);
        }

        internal static TipoValeResponse Map(TipoVale tipoVale) => new()
        {
            Code = tipoVale.Code,
            Name = tipoVale.Name,
            IsActive = tipoVale.IsActive,
            CreatedAt = tipoVale.CreatedAt,
            UpdatedAt = tipoVale.UpdatedAt,
            CreatedBy = tipoVale.CreatedBy,
            UpdatedBy = tipoVale.UpdatedBy,
            RowVersion = tipoVale.RowVersion
        };
    }
}