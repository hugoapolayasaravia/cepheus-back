using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.CreateTipoZona
{
    public class CreateTipoZonaCommandHandler : IRequestHandler<CreateTipoZonaCommand, TipoZonaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoZonaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoZonaResponse> Handle(CreateTipoZonaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposZona.Query().Select(t => t.Code), length: 3, entityLabel: "TiposZona", cancellationToken);

            var tipoZona = new TipoZona
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposZona.AddAsync(tipoZona, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoZona);
        }

        internal static TipoZonaResponse Map(TipoZona tipoZona) => new()
        {
            Code = tipoZona.Code,
            Name = tipoZona.Name,
            IsActive = tipoZona.IsActive,
            CreatedAt = tipoZona.CreatedAt,
            UpdatedAt = tipoZona.UpdatedAt,
            RowVersion = tipoZona.RowVersion
        };
    }
}