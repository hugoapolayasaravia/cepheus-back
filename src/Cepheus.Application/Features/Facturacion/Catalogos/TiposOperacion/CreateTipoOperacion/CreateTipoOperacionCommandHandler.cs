using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.CreateTipoOperacion
{
    public class CreateTipoOperacionCommandHandler
        : IRequestHandler<CreateTipoOperacionCommand, TipoOperacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoOperacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoOperacionResponse> Handle(
            CreateTipoOperacionCommand request,
            CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.TiposOperacion.Query().Select(t => t.Code),
                length: 2,
                entityLabel: "TiposOperacion",
                cancellationToken);

            var entity = new TipoOperacion
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.TiposOperacion.AddAsync(
                entity,
                cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static TipoOperacionResponse Map(TipoOperacion e) => new()
        {
            Code = e.Code,
            Name = e.Name,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}