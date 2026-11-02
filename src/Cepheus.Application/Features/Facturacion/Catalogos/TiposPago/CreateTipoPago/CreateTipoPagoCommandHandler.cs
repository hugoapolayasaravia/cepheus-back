using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.CreateTipoPago
{
    public class CreateTipoPagoCommandHandler : IRequestHandler<CreateTipoPagoCommand, TipoPagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoPagoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoPagoResponse> Handle(CreateTipoPagoCommand request, CancellationToken cancellationToken)
        {
            var accounts = request.Accounts ?? new List<TipoPagoCuentaInput>();
            await TipoPagoProjection.ValidateAccountsAsync(_uow, accounts, cancellationToken);

            var code = request.Code.Trim();

            var tipo = new TipoPago
            {
                Code = code,
                Name = request.Name.Trim(),
                SortOrder = request.SortOrder,
                ShowInApproval = request.ShowInApproval,
                IsActive = true,
                Accounts = accounts.Select(a => new TipoPagoCuentaContable
                {
                    TipoPagoCode = code,
                    MonedaCode = a.MonedaCode.Trim().ToUpperInvariant(),
                    AccountCode = a.AccountCode.Trim()
                }).ToList()
            };

            await _uow.Facturacion.Catalogos.TiposPago.AddAsync(tipo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _uow.ClearTracking();

            return (await TipoPagoProjection.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
