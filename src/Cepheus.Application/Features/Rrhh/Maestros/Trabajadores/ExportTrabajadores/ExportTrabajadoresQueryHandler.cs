using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.ExportTrabajadores
{
    public class ExportTrabajadoresQueryHandler : IRequestHandler<ExportTrabajadoresQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<TrabajadorResponse>> Catalogo = new List<ExportColumn<TrabajadorResponse>>
        {
            new("code", "Código", f => f.Code),
            new("firstNames", "Nombres", f => f.FirstNames),
            new("paternalSurname", "Apellido paterno", f => f.PaternalSurname),
            new("maternalSurname", "Apellido materno", f => f.MaternalSurname),
            new("sexoCode", "Sexo", f => f.SexoCode),
            new("estadoCivilCode", "Estado civil", f => f.EstadoCivilCode),
            new("nacionalidadCode", "Nacionalidad", f => f.NacionalidadCode),
            new("birthDate", "Fecha de nacimiento", f => f.BirthDate?.ToString("dd/MM/yyyy")),
            new("birthUbigeoCode", "Ubigeo de nacimiento", f => f.BirthUbigeoCode),
            new("email", "Correo", f => f.Email),
            new("phone", "Teléfono", f => f.Phone),
            new("mobilePhone", "Celular", f => f.MobilePhone),
            new("photoUrl", "Foto", f => f.PhotoUrl),
            new("hasDisability", "Con discapacidad", f => ExportBuilder.SiNo(f.HasDisability)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "firstNames", "paternalSurname", "maternalSurname", "sexoCode", "estadoCivilCode", "nacionalidadCode", "birthDate" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportTrabajadoresQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportTrabajadoresQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Rrhh.Maestros.Trabajadores.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(search) ||
                    (t.FirstNames != null && t.FirstNames.ToLower().Contains(search)) ||
                    (t.PaternalSurname != null && t.PaternalSurname.ToLower().Contains(search)) ||
                    (t.MaternalSurname != null && t.MaternalSurname.ToLower().Contains(search)) ||
                    (t.Email != null && t.Email.ToLower().Contains(search)));
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(t => new TrabajadorResponse
            {
                Code = t.Code,
                FirstNames = t.FirstNames,
                PaternalSurname = t.PaternalSurname,
                MaternalSurname = t.MaternalSurname,
                SexoCode = t.SexoCode,
                EstadoCivilCode = t.EstadoCivilCode,
                NacionalidadCode = t.NacionalidadCode,
                BirthDate = t.BirthDate,
                BirthUbigeoCode = t.BirthUbigeoCode,
                Email = t.Email,
                Phone = t.Phone,
                MobilePhone = t.MobilePhone,
                PhotoUrl = t.PhotoUrl,
                HasDisability = t.HasDisability,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
            });

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Trabajadores", "trabajadores", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, null));
        }
    }
}
