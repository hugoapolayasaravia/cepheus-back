using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.CreateTipoVia
{
    public class CreateTipoViaCommandHandler : IRequestHandler<CreateTipoViaCommand, TipoViaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoViaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoViaResponse> Handle(CreateTipoViaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposVia.Query().Select(t => t.Code), length: 3, entityLabel: "TiposVia", cancellationToken);

            var tipoVia = new TipoVia
            {
                Code = code,
                Name = request.Name.Trim(),
                Abbreviation = string.IsNullOrWhiteSpace(request.Abbreviation) ? null : request.Abbreviation.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposVia.AddAsync(tipoVia, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoVia);
        }

        internal static TipoViaResponse Map(TipoVia tipoVia) => new()
        {
            Code = tipoVia.Code,
            Name = tipoVia.Name,
            Abbreviation = tipoVia.Abbreviation,
            IsActive = tipoVia.IsActive,
            CreatedAt = tipoVia.CreatedAt,
            UpdatedAt = tipoVia.UpdatedAt,
            RowVersion = tipoVia.RowVersion
        };
    }
}