using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.CreateTipoExtensionContrato
{
    public class CreateTipoExtensionContratoCommandHandler
        : IRequestHandler<CreateTipoExtensionContratoCommand, TipoExtensionContratoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoExtensionContratoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoExtensionContratoResponse> Handle(CreateTipoExtensionContratoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposExtensionContrato.Query().Select(t => t.Code), length: 3, entityLabel: "TiposExtensionContrato", cancellationToken);

            var tipoExtensionContrato = new TipoExtensionContrato
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposExtensionContrato.AddAsync(tipoExtensionContrato, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoExtensionContrato);
        }

        internal static TipoExtensionContratoResponse Map(TipoExtensionContrato tipoExtensionContrato) => new()
        {
            Code = tipoExtensionContrato.Code,
            Name = tipoExtensionContrato.Name,
            IsActive = tipoExtensionContrato.IsActive,
            CreatedAt = tipoExtensionContrato.CreatedAt,
            UpdatedAt = tipoExtensionContrato.UpdatedAt,
            RowVersion = tipoExtensionContrato.RowVersion
        };
    }
}