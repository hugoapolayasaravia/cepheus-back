using System.Linq.Expressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.Common
{
    public class MotivoChequeReboteResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    public static class MotivoChequeReboteProjection
    {
        public static readonly Expression<Func<MotivoChequeRebote, MotivoChequeReboteResponse>> ToResponse =
            t => new MotivoChequeReboteResponse
            {
                Code = t.Code,
                Name = t.Name,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                RowVersion = t.RowVersion
            };

        public static Task<MotivoChequeReboteResponse?> LoadAsync(
            IUnitOfWork uow, string code, CancellationToken cancellationToken)
            => uow.Facturacion.Catalogos.MotivosChequeRebote.Query()
                .AsNoTracking()
                .Where(t => t.Code == code)
                .Select(ToResponse)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
