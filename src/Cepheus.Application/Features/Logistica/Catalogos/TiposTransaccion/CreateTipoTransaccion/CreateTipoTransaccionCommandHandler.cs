using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.CreateTipoTransaccion
{
    public class CreateTipoTransaccionCommandHandler
        : IRequestHandler<CreateTipoTransaccionCommand, TipoTransaccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoTransaccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoTransaccionResponse> Handle(
            CreateTipoTransaccionCommand request,
            CancellationToken cancellationToken)
        {
            var tipoTransaccion = new TipoTransaccion
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.TiposTransaccion.AddAsync(tipoTransaccion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoTransaccion);
        }

        internal static TipoTransaccionResponse Map(TipoTransaccion tipoTransaccion) => new()
        {
            Code = tipoTransaccion.Code,
            Name = tipoTransaccion.Name,
            IsActive = tipoTransaccion.IsActive,
            CreatedAt = tipoTransaccion.CreatedAt,
            UpdatedAt = tipoTransaccion.UpdatedAt,
            CreatedBy = tipoTransaccion.CreatedBy,
            UpdatedBy = tipoTransaccion.UpdatedBy,
            RowVersion = tipoTransaccion.RowVersion
        };
    }
}
