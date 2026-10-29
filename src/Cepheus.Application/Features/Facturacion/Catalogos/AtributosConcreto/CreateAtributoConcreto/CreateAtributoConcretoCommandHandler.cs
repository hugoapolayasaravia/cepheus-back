using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.CreateAtributoConcreto
{
    public class CreateAtributoConcretoCommandHandler
        : IRequestHandler<CreateAtributoConcretoCommand, AtributoConcretoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateAtributoConcretoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AtributoConcretoResponse> Handle(
            CreateAtributoConcretoCommand request,
            CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.AtributosConcreto.Query().Select(a => a.Code),
                length: 4,
                entityLabel: "AtributosConcreto",
                cancellationToken);

            var entity = new AtributoConcreto
            {
                Code = code,
                AttributeType = System.Enum.Parse<TipoAtributoConcreto>(
                    request.AttributeType.Trim(),
                    ignoreCase: true),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.AtributosConcreto.AddAsync(
                entity,
                cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static AtributoConcretoResponse Map(AtributoConcreto e) => new()
        {
            Code = e.Code,
            AttributeType = e.AttributeType.ToString(),
            Name = e.Name,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}