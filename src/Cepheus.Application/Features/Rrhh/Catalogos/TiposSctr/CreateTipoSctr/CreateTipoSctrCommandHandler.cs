using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.CreateTipoSctr
{
    public class CreateTipoSctrCommandHandler : IRequestHandler<CreateTipoSctrCommand, TipoSctrResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoSctrCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoSctrResponse> Handle(CreateTipoSctrCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposSctr.Query().Select(t => t.Code), length: 3, entityLabel: "TiposSctr", cancellationToken);

            var tipoSctr = new TipoSctr
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposSctr.AddAsync(tipoSctr, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoSctr);
        }

        internal static TipoSctrResponse Map(TipoSctr tipoSctr) => new()
        {
            Code = tipoSctr.Code,
            Name = tipoSctr.Name,
            IsActive = tipoSctr.IsActive,
            CreatedAt = tipoSctr.CreatedAt,
            UpdatedAt = tipoSctr.UpdatedAt,
            RowVersion = tipoSctr.RowVersion
        };
    }
}