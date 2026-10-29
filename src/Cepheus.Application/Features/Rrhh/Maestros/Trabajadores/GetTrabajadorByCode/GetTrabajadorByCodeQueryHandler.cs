using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.GetTrabajadorByCode
{
    public class GetTrabajadorByCodeQueryHandler : IRequestHandler<GetTrabajadorByCodeQuery, TrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorResponse> Handle(GetTrabajadorByCodeQuery request, CancellationToken cancellationToken)
        {
            var trabajador = await _uow.Rrhh.Maestros.Trabajadores.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TrabajadorResponse
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
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (trabajador is null)
            {
                throw new KeyNotFoundException($"Trabajador {request.Code} no encontrado.");
            }

            return trabajador;
        }
    }
}