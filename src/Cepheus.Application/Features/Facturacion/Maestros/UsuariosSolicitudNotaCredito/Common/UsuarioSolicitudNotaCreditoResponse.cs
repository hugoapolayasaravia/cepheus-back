using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Common
{
    public class UsuarioSolicitudNotaCreditoResponse
    {
        public string NegocioCode { get; set; } = default!;
        public string? NegocioName { get; set; }

        [JsonIgnore]
        public EtapaSolicitudNotaCredito StageValue { get; set; }
        public int Stage => (int)StageValue;
        public string StageName => StageValue.ToString();

        public int UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }

        public bool IsPrincipal { get; set; }
        public bool CanEnter { get; set; }
        public bool CanManage { get; set; }
        public bool CanApprove { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    public static class UsuarioSolicitudNotaCreditoProjection
    {
        public static readonly Expression<Func<UsuarioSolicitudNotaCredito, UsuarioSolicitudNotaCreditoResponse>> ToResponse =
            t => new UsuarioSolicitudNotaCreditoResponse
            {
                NegocioCode = t.NegocioCode,
                NegocioName = t.Negocio.Name,
                StageValue = t.Stage,
                UserId = t.UserId,
                UserFullName = t.User.FirstName + " " + t.User.LastName,
                UserEmail = t.User.Email,
                IsPrincipal = t.IsPrincipal,
                CanEnter = t.CanEnter,
                CanManage = t.CanManage,
                CanApprove = t.CanApprove,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
            };

        public static Task<UsuarioSolicitudNotaCreditoResponse?> LoadAsync(
            IUnitOfWork uow, string negocioCode, EtapaSolicitudNotaCredito stage, int userId,
            CancellationToken cancellationToken)
            => uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Query()
                .AsNoTracking()
                .Where(t => t.NegocioCode == negocioCode && t.Stage == stage && t.UserId == userId)
                .Select(ToResponse)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
