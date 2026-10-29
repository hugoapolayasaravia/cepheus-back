using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Monedas.Common;
using Cepheus.Domain.Comunes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Monedas.UpdateMoneda
{
    public class UpdateMonedaCommandHandler : IRequestHandler<UpdateMonedaCommand, MonedaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateMonedaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MonedaResponse> Handle(UpdateMonedaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Comunes.Monedas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Moneda {request.Code} no encontrada.");
            }

            var moneda = new Moneda
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                Symbol = string.IsNullOrWhiteSpace(request.Symbol) ? null : request.Symbol.Trim(),
                NumericCode = string.IsNullOrWhiteSpace(request.NumericCode) ? null : request.NumericCode.Trim(),
                DecimalPlaces = request.DecimalPlaces,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Comunes.Monedas.Update(moneda);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La moneda fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new MonedaResponse
            {
                Code = moneda.Code,
                Name = moneda.Name,
                Symbol = moneda.Symbol,
                NumericCode = moneda.NumericCode,
                DecimalPlaces = moneda.DecimalPlaces,
                IsActive = moneda.IsActive,
                CreatedAt = moneda.CreatedAt,
                UpdatedAt = moneda.UpdatedAt,
                CreatedBy = moneda.CreatedBy,
                UpdatedBy = moneda.UpdatedBy,
                RowVersion = moneda.RowVersion
            };
        }
    }
}
