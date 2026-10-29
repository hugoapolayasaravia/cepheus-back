using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.TiposCambio.CreateTipoCambio
{
    public class CreateTipoCambioCommandHandler : IRequestHandler<CreateTipoCambioCommand, TipoCambioResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoCambioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCambioResponse> Handle(CreateTipoCambioCommand request, CancellationToken cancellationToken)
        {
            var tipoCambio = new TipoCambio
            {
                MonedaCode = TipoCambioRules.Normalizar(request.MonedaCode),
                Date = request.Date,
                SellRate = request.SellRate,
                BuyRate = request.BuyRate
            };

            await _uow.Comunes.TiposCambio.AddAsync(tipoCambio, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

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
