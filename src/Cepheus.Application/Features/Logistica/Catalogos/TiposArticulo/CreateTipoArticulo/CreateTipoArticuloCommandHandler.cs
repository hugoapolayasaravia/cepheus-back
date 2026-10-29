using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.CreateTipoArticulo
{
    public class CreateTipoArticuloCommandHandler : IRequestHandler<CreateTipoArticuloCommand, TipoArticuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoArticuloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoArticuloResponse> Handle(CreateTipoArticuloCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.TiposArticulo.Query().Select(t => t.Code), length: 3, entityLabel: "Tipos de Artículo", cancellationToken);

            var tipoArticulo = new TipoArticulo
            {
                Code = code,
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.TiposArticulo.AddAsync(tipoArticulo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoArticulo);
        }

        internal static TipoArticuloResponse Map(TipoArticulo tipoArticulo) => new()
        {
            Code = tipoArticulo.Code,
            Name = tipoArticulo.Name,
            Description = tipoArticulo.Description,
            IsActive = tipoArticulo.IsActive,
            CreatedAt = tipoArticulo.CreatedAt,
            UpdatedAt = tipoArticulo.UpdatedAt,
            CreatedBy = tipoArticulo.CreatedBy,
            UpdatedBy = tipoArticulo.UpdatedBy,
            RowVersion = tipoArticulo.RowVersion
        };
    }
}