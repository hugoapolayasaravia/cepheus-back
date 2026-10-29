using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Cepheus.Domain.Comunes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.TiposCambio.UpdateTipoCambio
{
    public class UpdateTipoCambioCommandHandler : IRequestHandler<UpdateTipoCambioCommand, TipoCambioResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoCambioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCambioResponse> Handle(UpdateTipoCambioCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Comunes.TiposCambio.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de cambio {request.Id} no encontrado.");
            }

            var tipoCambio = new TipoCambio
            {
                Id = request.Id,
                MonedaCode = TipoCambioRules.Normalizar(request.MonedaCode),
                Date = request.Date,
                SellRate = request.SellRate,
                BuyRate = request.BuyRate,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Comunes.TiposCambio.Update(tipoCambio);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de cambio fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return await _uow.Comunes.TiposCambio.Query()
                .AsNoTracking()
                .Where(t => t.Id == tipoCambio.Id)
                .Select(t => new TipoCambioResponse
                {
                    Id = t.Id,
                    MonedaCode = t.MonedaCode,
                    MonedaName = t.Moneda.Name,
                    Date = t.Date,
                    SellRate = t.SellRate,
                    BuyRate = t.BuyRate,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    CreatedBy = t.CreatedBy,
                    UpdatedBy = t.UpdatedBy,
                    RowVersion = t.RowVersion
                })
                .FirstAsync(cancellationToken);
        }
    }
}
