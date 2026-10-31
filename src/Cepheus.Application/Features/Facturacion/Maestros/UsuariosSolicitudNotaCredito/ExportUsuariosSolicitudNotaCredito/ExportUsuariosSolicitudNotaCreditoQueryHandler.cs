using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.ExportUsuariosSolicitudNotaCredito
{
    public class ExportUsuariosSolicitudNotaCreditoQueryHandler : IRequestHandler<ExportUsuariosSolicitudNotaCreditoQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<UsuarioSolicitudNotaCreditoResponse>> Catalogo = new List<ExportColumn<UsuarioSolicitudNotaCreditoResponse>>
        {
            new("negocioCode", "Negocio", f => f.NegocioCode),
            new("negocioName", "Nombre del negocio", f => f.NegocioName),
            new("stageValue", "Etapa", f => f.StageValue.ToString()),
            new("userId", "Usuario (Id)", f => f.UserId),
            new("userFullName", "Nombre del usuario", f => f.UserFullName),
            new("userEmail", "Correo del usuario", f => f.UserEmail),
            new("isPrincipal", "Principal", f => ExportBuilder.SiNo(f.IsPrincipal)),
            new("canEnter", "Puede ingresar", f => ExportBuilder.SiNo(f.CanEnter)),
            new("canManage", "Puede gestionar", f => ExportBuilder.SiNo(f.CanManage)),
            new("canApprove", "Puede aprobar", f => ExportBuilder.SiNo(f.CanApprove)),
            new("startDate", "Inicio", f => f.StartDate?.ToString("dd/MM/yyyy")),
            new("endDate", "Fin", f => f.EndDate?.ToString("dd/MM/yyyy")),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "negocioCode", "negocioName", "stageValue", "userId", "userFullName", "userEmail", "isPrincipal", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportUsuariosSolicitudNotaCreditoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportUsuariosSolicitudNotaCreditoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.NegocioCode))
            {
                var negocio = request.NegocioCode.Trim().ToUpperInvariant();
                query = query.Where(u => u.NegocioCode == negocio);
            }

            if (!string.IsNullOrWhiteSpace(request.Stage))
            {
                var stage = EtapaSolicitudNotaCreditoParser.Parse(request.Stage);
                query = query.Where(u => u.Stage == stage);
            }

            if (request.UserId.HasValue)
            {
                query = query.Where(u => u.UserId == request.UserId.Value);
            }

            if (request.IsPrincipal.HasValue)
            {
                query = query.Where(u => u.IsPrincipal == request.IsPrincipal.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.User.FirstName.ToLower().Contains(search) ||
                    u.User.LastName.ToLower().Contains(search) ||
                    u.User.Email.ToLower().Contains(search) ||
                    u.Negocio.Name.ToLower().Contains(search));
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(u => u.NegocioCode).ThenBy(u => u.Stage).ThenBy(u => u.UserId)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(UsuarioSolicitudNotaCreditoProjection.ToResponse);

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Usuarios de solicitud de nota de crédito", "usuarios_solicitud_nota_credito", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.NegocioCode) ? null : $"Negocio: {request.NegocioCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.Stage) ? null : $"Etapa: {request.Stage.Trim()}",
                    request.UserId.HasValue ? $"Usuario (Id): {request.UserId.Value}" : null,
                    request.IsPrincipal.HasValue ? $"Principal: {ExportBuilder.SiNo(request.IsPrincipal.Value)}" : null));
        }
    }
}
