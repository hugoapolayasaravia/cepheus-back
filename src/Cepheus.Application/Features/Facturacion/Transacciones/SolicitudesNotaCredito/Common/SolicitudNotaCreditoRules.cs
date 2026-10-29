using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common
{
    public static class SolicitudNotaCreditoRules
    {
        public const int MaxLines = 999;
        public const int CodeLength = 8;

        /// <summary>Etapas que se gestionan (aprueban/rechazan). Ingresa es el registro y no se aprueba.</summary>
        public static readonly EtapaSolicitudNotaCredito[] ApprovalStages =
        {
            EtapaSolicitudNotaCredito.Ventas,
            EtapaSolicitudNotaCredito.CreditosGestiona,
            EtapaSolicitudNotaCredito.CreditosAprueba,
            EtapaSolicitudNotaCredito.Contabilidad,
            EtapaSolicitudNotaCredito.UnidadNegocio
        };

        public static string Normalize(string value) => value.Trim().ToUpperInvariant();

        /// <summary>
        /// Facultad que exige cada etapa en Usuarios de Solicitud (MAE_USER_GESTION_SNC):
        /// Ingresa -> CanEnter; CreditosGestiona -> CanManage; resto -> CanApprove.
        /// </summary>
        public static Func<UsuarioSolicitudNotaCredito, bool> RequiredFlag(EtapaSolicitudNotaCredito etapa)
            => etapa switch
            {
                EtapaSolicitudNotaCredito.Ingresa => u => u.CanEnter,
                EtapaSolicitudNotaCredito.CreditosGestiona => u => u.CanManage,
                _ => u => u.CanApprove
            };

        /// <summary>
        /// El usuario autenticado debe estar asignado (activo y vigente) a la etapa del negocio
        /// con la facultad que la etapa exige.
        /// </summary>
        public static async Task EnsureUserCanAsync(
            IUnitOfWork uow, ICurrentUserService currentUser, string negocioCode,
            EtapaSolicitudNotaCredito etapa, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not int userId)
            {
                throw new InvalidOperationException("No se pudo identificar al usuario autenticado.");
            }

            var today = DateTime.UtcNow.Date;

            var asignaciones = await uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Query()
                .AsNoTracking()
                .Where(u => u.NegocioCode == negocioCode && u.Stage == etapa && u.UserId == userId
                            && u.IsActive
                            && (u.StartDate == null || u.StartDate <= today)
                            && (u.EndDate == null || u.EndDate >= today))
                .ToListAsync(cancellationToken);

            if (!asignaciones.Any(RequiredFlag(etapa)))
            {
                throw new InvalidOperationException(
                    $"El usuario no está autorizado en la etapa {etapa} del negocio {negocioCode}. " +
                    "Verifique Usuarios de Solicitud de Nota de Crédito.");
            }
        }

        public static (List<SolicitudNotaCreditoDetalle> Details, decimal SubTotal) BuildDetails(
            string code, string year, string month, IReadOnlyList<SolicitudNotaCreditoDetalleInput> inputs)
        {
            var details = new List<SolicitudNotaCreditoDetalle>(inputs.Count);

            for (var i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                var lineNumber = i + 1;

                details.Add(new SolicitudNotaCreditoDetalle
                {
                    Code = code,
                    Year = year,
                    Month = month,
                    TipoProductoCode = input.TipoProductoCode.Trim(),
                    ProductoCode = input.ProductoCode.Trim(),
                    LineKey = input.LineKey ?? lineNumber,
                    Line = lineNumber.ToString().PadLeft(3, '0'),
                    Quantity = input.Quantity,
                    UnitPrice = input.UnitPrice,
                    Total = DocumentoVentaRules.Round2(input.Quantity * input.UnitPrice),
                    OrderCode = string.IsNullOrWhiteSpace(input.OrderCode) ? null : input.OrderCode.Trim()
                });
            }

            return (details, details.Sum(d => d.Total));
        }

        public static List<SolicitudNotaCreditoGuia> BuildGuides(
            string code, string year, string month, IReadOnlyList<SolicitudNotaCreditoGuiaInput>? inputs)
            => (inputs ?? new List<SolicitudNotaCreditoGuiaInput>())
                .Select(g => new SolicitudNotaCreditoGuia
                {
                    Code = code,
                    Year = year,
                    Month = month,
                    GuideNumber = g.GuideNumber.Trim(),
                    Quantity = g.Quantity
                }).ToList();

        public static List<SolicitudNotaCreditoAprobacion> BuildApprovals(string code, string year, string month)
            => ApprovalStages
                .Select(stage => new SolicitudNotaCreditoAprobacion
                {
                    Code = code,
                    Year = year,
                    Month = month,
                    Stage = stage,
                    Status = EstadoAprobacionSolicitud.Pendiente
                }).ToList();

        /// <summary>Siguiente correlativo del año (8 dígitos), máximo existente + 1.</summary>
        public static async Task<string> NextCodeAsync(
            IUnitOfWork uow, string year, CancellationToken cancellationToken)
        {
            var lastCode = await uow.Facturacion.Transacciones.SolicitudesNotaCredito.Query()
                .AsNoTracking()
                .Where(s => s.Year == year)
                .OrderByDescending(s => s.Code)
                .Select(s => s.Code)
                .FirstOrDefaultAsync(cancellationToken);

            var next = 1;
            if (lastCode is not null && int.TryParse(lastCode, out var lastNumber))
            {
                next = lastNumber + 1;
            }

            return next.ToString().PadLeft(CodeLength, '0');
        }

        public static string FormatKey(string year, string month, string code) => $"{year}/{month}/{code}";
    }
}
