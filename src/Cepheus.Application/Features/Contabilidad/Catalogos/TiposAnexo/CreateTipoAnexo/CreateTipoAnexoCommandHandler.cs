using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.Common;
using Cepheus.Domain.Contabilidad.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.CreateTipoAnexo
{
    public class CreateTipoAnexoCommandHandler
        : IRequestHandler<CreateTipoAnexoCommand, TipoAnexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoAnexoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoAnexoResponse> Handle(
            CreateTipoAnexoCommand request,
            CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Contabilidad.Catalogos.TiposAnexo.Query().Select(t => t.Code),
                length: 3,
                entityLabel: "TiposAnexo",
                cancellationToken);

            var entity = new TipoAnexo
            {
                Code = code,
                Name = request.Name.Trim(),
                Flag = string.IsNullOrWhiteSpace(request.Flag) ? null : request.Flag.Trim(),
                IsActive = true
            };

            await _uow.Contabilidad.Catalogos.TiposAnexo.AddAsync(
                entity,
                cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static TipoAnexoResponse Map(TipoAnexo e) => new()
        {
            Code = e.Code,
            Name = e.Name,
            Flag = e.Flag,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
