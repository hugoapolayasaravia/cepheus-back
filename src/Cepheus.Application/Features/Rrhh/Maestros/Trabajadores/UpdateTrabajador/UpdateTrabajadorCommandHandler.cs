using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.UpdateTrabajador
{
    public class UpdateTrabajadorCommandHandler : IRequestHandler<UpdateTrabajadorCommand, TrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorResponse> Handle(UpdateTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.Trabajadores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Trabajador {request.Code} no encontrado.");
            }

            var trabajador = new Trabajador
            {
                Code = request.Code,
                FirstNames = string.IsNullOrWhiteSpace(request.FirstNames) ? null : request.FirstNames.Trim(),
                PaternalSurname = string.IsNullOrWhiteSpace(request.PaternalSurname) ? null : request.PaternalSurname.Trim(),
                MaternalSurname = string.IsNullOrWhiteSpace(request.MaternalSurname) ? null : request.MaternalSurname.Trim(),
                SexoCode = string.IsNullOrWhiteSpace(request.SexoCode) ? null : request.SexoCode.Trim(),
                EstadoCivilCode = string.IsNullOrWhiteSpace(request.EstadoCivilCode) ? null : request.EstadoCivilCode.Trim(),
                NacionalidadCode = string.IsNullOrWhiteSpace(request.NacionalidadCode) ? null : request.NacionalidadCode.Trim(),
                BirthDate = request.BirthDate,
                BirthUbigeoCode = string.IsNullOrWhiteSpace(request.BirthUbigeoCode) ? null : request.BirthUbigeoCode.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                MobilePhone = string.IsNullOrWhiteSpace(request.MobilePhone) ? null : request.MobilePhone.Trim(),
                PhotoUrl = string.IsNullOrWhiteSpace(request.PhotoUrl) ? null : request.PhotoUrl.Trim(),
                HasDisability = request.HasDisability,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.Trabajadores.Update(trabajador);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El trabajador fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TrabajadorResponse
            {
                Code = trabajador.Code,
                FirstNames = trabajador.FirstNames,
                PaternalSurname = trabajador.PaternalSurname,
                MaternalSurname = trabajador.MaternalSurname,
                SexoCode = trabajador.SexoCode,
                EstadoCivilCode = trabajador.EstadoCivilCode,
                NacionalidadCode = trabajador.NacionalidadCode,
                BirthDate = trabajador.BirthDate,
                BirthUbigeoCode = trabajador.BirthUbigeoCode,
                Email = trabajador.Email,
                Phone = trabajador.Phone,
                MobilePhone = trabajador.MobilePhone,
                PhotoUrl = trabajador.PhotoUrl,
                HasDisability = trabajador.HasDisability,
                CreatedAt = trabajador.CreatedAt,
                UpdatedAt = trabajador.UpdatedAt,
                RowVersion = trabajador.RowVersion
            };
        }
    }
}