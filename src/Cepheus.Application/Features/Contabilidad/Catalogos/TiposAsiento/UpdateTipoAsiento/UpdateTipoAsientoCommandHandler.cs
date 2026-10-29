using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common;
using Cepheus.Domain.Contabilidad.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.UpdateTipoAsiento
{
    public class UpdateTipoAsientoCommandHandler : IRequestHandler<UpdateTipoAsientoCommand, TipoAsientoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoAsientoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoAsientoResponse> Handle(UpdateTipoAsientoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Contabilidad.Catalogos.TiposAsiento.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de asiento {request.Code} no encontrado.");
            }

            var entity = new TipoAsiento
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                ExchangeRateType = request.ExchangeRateType,
                Valuation = request.Valuation,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Contabilidad.Catalogos.TiposAsiento.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTipoAsiento.CreateTipoAsientoCommandHandler.Map(entity);
        }
    }
}
