using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.CreateTipoContrato
{
    public class CreateTipoContratoCommandHandler : IRequestHandler<CreateTipoContratoCommand, TipoContratoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoContratoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoContratoResponse> Handle(CreateTipoContratoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposContrato.Query().Select(t => t.Code), length: 3, entityLabel: "TiposContrato", cancellationToken);

            var tipoContrato = new TipoContrato
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposContrato.AddAsync(tipoContrato, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoContrato);
        }

        internal static TipoContratoResponse Map(TipoContrato tipoContrato) => new()
        {
            Code = tipoContrato.Code,
            Name = tipoContrato.Name,
            IsActive = tipoContrato.IsActive,
            CreatedAt = tipoContrato.CreatedAt,
            UpdatedAt = tipoContrato.UpdatedAt,
            RowVersion = tipoContrato.RowVersion
        };
    }
}