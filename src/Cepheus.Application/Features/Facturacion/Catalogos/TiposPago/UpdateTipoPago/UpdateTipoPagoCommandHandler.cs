using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.UpdateTipoPago
{
    public class UpdateTipoPagoCommandHandler : IRequestHandler<UpdateTipoPagoCommand, TipoPagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoPagoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoPagoResponse> Handle(UpdateTipoPagoCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();
            var accounts = request.Accounts ?? new List<TipoPagoCuentaInput>();

            await TipoPagoProjection.ValidateAccountsAsync(_uow, accounts, cancellationToken);

            var tipo = await _uow.Facturacion.Catalogos.TiposPago.Query()
                .Include(t => t.Accounts)
                .FirstOrDefaultAsync(t => t.Code == code, cancellationToken)
                ?? throw new KeyNotFoundException($"Tipo de pago {request.Code} no encontrado.");

            if (!tipo.RowVersion.SequenceEqual(request.RowVersion))
            {
                throw new InvalidOperationException(
                    "El tipo de pago fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            tipo.Name = request.Name.Trim();
            tipo.SortOrder = request.SortOrder;
            tipo.ShowInApproval = request.ShowInApproval;

            // Reemplazo de cuentas: actualiza las que existen, agrega las nuevas y quita las que ya no vienen.
            var wanted = accounts.ToDictionary(a => a.MonedaCode.Trim().ToUpperInvariant(), a => a.AccountCode.Trim());

            foreach (var existing in tipo.Accounts.ToList())
            {
                if (wanted.TryGetValue(existing.MonedaCode, out var account))
                {
                    existing.AccountCode = account;
                    wanted.Remove(existing.MonedaCode);
                }
                else
                {
                    tipo.Accounts.Remove(existing);
                }
            }

            foreach (var (moneda, account) in wanted)
            {
                tipo.Accounts.Add(new TipoPagoCuentaContable { TipoPagoCode = code, MonedaCode = moneda, AccountCode = account });
            }

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de pago fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await TipoPagoProjection.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
