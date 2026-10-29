using System.Linq.Expressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common
{
    public class MotivoSolicitudNotaCreditoResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? ParentCode { get; set; }
        public string? ParentName { get; set; }
        public string? MotivoDevolucionCode { get; set; }
        public string? MotivoDevolucionName { get; set; }
        public bool AffectsStock { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    public static class MotivoSolicitudNotaCreditoProjection
    {
        public static readonly Expression<Func<MotivoSolicitudNotaCredito, MotivoSolicitudNotaCreditoResponse>> ToResponse =
            t => new MotivoSolicitudNotaCreditoResponse
            {
                Code = t.Code,
                Name = t.Name,
                ParentCode = t.ParentCode,
                ParentName = t.Parent != null ? t.Parent.Name : null,
                MotivoDevolucionCode = t.MotivoDevolucionCode,
                MotivoDevolucionName = t.MotivoDevolucion != null ? t.MotivoDevolucion.Name : null,
                AffectsStock = t.AffectsStock,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
            };

        public static Task<MotivoSolicitudNotaCreditoResponse?> LoadAsync(
            IUnitOfWork uow, string code, CancellationToken cancellationToken)
            => uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query()
                .AsNoTracking()
                .Where(t => t.Code == code)
                .Select(ToResponse)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
