using System.Linq.Expressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.Common
{
    public class TipoPagoCuentaResponse
    {
        public string MonedaCode { get; set; } = default!;
        public string AccountCode { get; set; } = default!;
        public string? AccountName { get; set; }
    }

    public class TipoPagoResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int SortOrder { get; set; }
        public bool ShowInApproval { get; set; }
        public bool IsActive { get; set; }
        public List<TipoPagoCuentaResponse> Accounts { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    public class TipoPagoCuentaInput
    {
        public string MonedaCode { get; set; } = default!;
        public string AccountCode { get; set; } = default!;
    }

    public static class TipoPagoProjection
    {
        public static readonly Expression<Func<TipoPago, TipoPagoResponse>> ToResponse =
            t => new TipoPagoResponse
            {
                Code = t.Code,
                Name = t.Name,
                SortOrder = t.SortOrder,
                ShowInApproval = t.ShowInApproval,
                IsActive = t.IsActive,
                Accounts = t.Accounts
                    .OrderBy(a => a.MonedaCode)
                    .Select(a => new TipoPagoCuentaResponse
                    {
                        MonedaCode = a.MonedaCode,
                        AccountCode = a.AccountCode,
                        AccountName = a.Account.Name
                    })
                    .ToList(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
            };

        public static Task<TipoPagoResponse?> LoadAsync(IUnitOfWork uow, string code, CancellationToken cancellationToken)
            => uow.Facturacion.Catalogos.TiposPago.Query()
                .AsNoTracking()
                .Where(t => t.Code == code)
                .Select(ToResponse)
                .FirstOrDefaultAsync(cancellationToken);

        /// <summary>Valida las cuentas informadas: moneda activa, cuenta existente de 8 caracteres y sin monedas repetidas.</summary>
        public static async Task ValidateAccountsAsync(
            IUnitOfWork uow, IReadOnlyList<TipoPagoCuentaInput> accounts, CancellationToken cancellationToken)
        {
            if (accounts.GroupBy(a => a.MonedaCode.Trim().ToUpperInvariant()).Any(g => g.Count() > 1))
            {
                throw new InvalidOperationException("Hay una moneda repetida en las cuentas del tipo de pago.");
            }

            foreach (var a in accounts)
            {
                var moneda = a.MonedaCode.Trim().ToUpperInvariant();
                var cuenta = a.AccountCode.Trim();

                if (!await uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == moneda && m.IsActive, cancellationToken))
                {
                    throw new InvalidOperationException($"La moneda {moneda} no existe o está inactiva.");
                }

                if (!await uow.Contabilidad.Maestros.CuentasContables.Query().AnyAsync(c => c.Code == cuenta, cancellationToken))
                {
                    throw new InvalidOperationException($"La cuenta contable {cuenta} no está registrada.");
                }
            }
        }
    }
}
