using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.ExportTransportistas
{
    public class ExportTransportistasQueryHandler : IRequestHandler<ExportTransportistasQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<TransportistaResponse>> Catalogo = new List<ExportColumn<TransportistaResponse>>
        {
            new("code", "Código", f => f.Code),
            new("documentTypeCode", "Tipo de documento", f => f.DocumentTypeCode),
            new("documentNumber", "Número de documento", f => f.DocumentNumber),
            new("legalName", "Razón social", f => f.LegalName),
            new("tradeName", "Nombre comercial", f => f.TradeName),
            new("address", "Dirección", f => f.Address),
            new("ubigeoCode", "Ubigeo", f => f.UbigeoCode),
            new("phone", "Teléfono", f => f.Phone),
            new("email", "Correo", f => f.Email),
            new("mtcRegistrationNumber", "Registro MTC", f => f.MtcRegistrationNumber),
            new("certificationNumber", "Certificación", f => f.CertificationNumber),
            new("isOwnFleet", "Flota propia", f => ExportBuilder.SiNo(f.IsOwnFleet)),
            new("observations", "Observaciones", f => f.Observations),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "documentNumber", "legalName", "phone", "isOwnFleet", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportTransportistasQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportTransportistasQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Logistica.Maestros.Transportistas.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t => t.Code.ToLower().Contains(search) || t.DocumentNumber.ToLower().Contains(search) || t.LegalName.ToLower().Contains(search) || (t.TradeName != null && t.TradeName.ToLower().Contains(search)));
            }

            if (request.IsOwnFleet.HasValue)
            {
                query = query.Where(t => t.IsOwnFleet == request.IsOwnFleet.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(t => new TransportistaResponse
                {
                Code = t.Code,
                DocumentTypeCode = t.DocumentTypeCode,
                DocumentNumber = t.DocumentNumber,
                LegalName = t.LegalName,
                TradeName = t.TradeName,
                Address = t.Address,
                UbigeoCode = t.UbigeoCode,
                Phone = t.Phone,
                Email = t.Email,
                MtcRegistrationNumber = t.MtcRegistrationNumber,
                CertificationNumber = t.CertificationNumber,
                IsOwnFleet = t.IsOwnFleet,
                Observations = t.Observations,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
                })
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Transportistas", "transportistas", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                request.IsOwnFleet.HasValue ? (request.IsOwnFleet.Value ? "Flota propia" : "Flota de terceros") : null));
        }
    }
}
