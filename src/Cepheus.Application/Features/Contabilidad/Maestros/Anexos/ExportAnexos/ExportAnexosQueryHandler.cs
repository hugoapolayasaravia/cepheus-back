using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.CreateAnexo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.ExportAnexos
{
    public class ExportAnexosQueryHandler : IRequestHandler<ExportAnexosQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<AnexoResponse>> Catalogo = new List<ExportColumn<AnexoResponse>>
        {
            new("tipoAnexoCode", "Tipo de anexo", f => f.TipoAnexoCode),
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("linkAccount1Code", "Cuenta de enlace 1", f => f.LinkAccount1Code),
            new("linkAccount2Code", "Cuenta de enlace 2", f => f.LinkAccount2Code),
            new("flag", "Indicador", f => f.Flag),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "tipoAnexoCode", "code", "name", "linkAccount1Code", "linkAccount2Code", "flag", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportAnexosQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportAnexosQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Contabilidad.Maestros.Anexos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    (x.Name != null && x.Name.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.TipoAnexoCode))
            {
                var tipo = request.TipoAnexoCode.Trim();
                query = query.Where(x => x.TipoAnexoCode == tipo);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.TipoAnexoCode).ThenBy(x => x.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(CreateAnexoCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);
            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateAnexoCommandHandler.Map)
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Anexos", "anexos", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.TipoAnexoCode) ? null : $"Tipo de anexo: {request.TipoAnexoCode.Trim()}"));
        }
    }
}
