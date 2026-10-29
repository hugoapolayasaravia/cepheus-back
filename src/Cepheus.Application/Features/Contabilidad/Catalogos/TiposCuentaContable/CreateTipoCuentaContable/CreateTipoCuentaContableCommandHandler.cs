using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.Common;
using Cepheus.Domain.Contabilidad.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.CreateTipoCuentaContable
{
    public class CreateTipoCuentaContableCommandHandler
        : IRequestHandler<CreateTipoCuentaContableCommand, TipoCuentaContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoCuentaContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCuentaContableResponse> Handle(
            CreateTipoCuentaContableCommand request,
            CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Contabilidad.Catalogos.TiposCuentaContable.Query().Select(t => t.Code),
                length: 2,
                entityLabel: "TiposCuentaContable",
                cancellationToken);

            var entity = new TipoCuentaContable
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Contabilidad.Catalogos.TiposCuentaContable.AddAsync(
                entity,
                cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static TipoCuentaContableResponse Map(TipoCuentaContable e) => new()
        {
            Code = e.Code,
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
