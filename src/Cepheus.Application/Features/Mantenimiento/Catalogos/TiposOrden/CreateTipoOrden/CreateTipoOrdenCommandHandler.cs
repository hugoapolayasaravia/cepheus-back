using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.CreateTipoOrden
{
    public class CreateTipoOrdenCommandHandler
        : IRequestHandler<CreateTipoOrdenCommand, TipoOrdenResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoOrdenCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoOrdenResponse> Handle(
            CreateTipoOrdenCommand request,
            CancellationToken cancellationToken)
        {
            var tipoOrden = new TipoOrden
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Mantenimiento.Catalogos.TiposOrden.AddAsync(tipoOrden, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoOrden);
        }

        internal static TipoOrdenResponse Map(TipoOrden tipoOrden) => new()
        {
            Code = tipoOrden.Code,
            Name = tipoOrden.Name,
            IsActive = tipoOrden.IsActive,
            CreatedAt = tipoOrden.CreatedAt,
            UpdatedAt = tipoOrden.UpdatedAt,
            RowVersion = tipoOrden.RowVersion
        };
    }
}
