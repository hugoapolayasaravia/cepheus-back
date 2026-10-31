using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.CreateAtributoConcreto;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.ExportAtributosConcreto
{
    public class ExportAtributosConcretoQueryHandler : IRequestHandler<ExportAtributosConcretoQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<AtributoConcretoResponse>> Catalogo = new List<ExportColumn<AtributoConcretoResponse>>
        {
            new("code", "Código", f => f.Code),
            new("attributeType", "Tipo de atributo", f => f.AttributeType),
            new("name", "Nombre", f => f.Name),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "attributeType", "name", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportAtributosConcretoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportAtributosConcretoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.AtributosConcreto.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.AttributeType) &&
                System.Enum.TryParse<TipoAtributoConcreto>(request.AttributeType.Trim(), true, out var attributeType) &&
                System.Enum.IsDefined(attributeType))
            {
                query = query.Where(x => x.AttributeType == attributeType);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(CreateAtributoConcretoCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);
            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateAtributoConcretoCommandHandler.Map)
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Atributos de concreto", "atributos_concreto", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.AttributeType) ? null : $"Tipo de atributo: {request.AttributeType.Trim()}"));
        }
    }
}
