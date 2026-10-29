using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Monedas.Common;
using Cepheus.Domain.Comunes;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Monedas.CreateMoneda
{
    public class CreateMonedaCommandHandler : IRequestHandler<CreateMonedaCommand, MonedaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateMonedaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MonedaResponse> Handle(CreateMonedaCommand request, CancellationToken cancellationToken)
        {
            var moneda = new Moneda
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                Symbol = string.IsNullOrWhiteSpace(request.Symbol) ? null : request.Symbol.Trim(),
                NumericCode = string.IsNullOrWhiteSpace(request.NumericCode) ? null : request.NumericCode.Trim(),
                DecimalPlaces = request.DecimalPlaces,
                IsActive = true
            };

            await _uow.Comunes.Monedas.AddAsync(moneda, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(moneda);
        }

        internal static MonedaResponse Map(Moneda moneda) => new()
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
