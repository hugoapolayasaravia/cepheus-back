using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.CreateTrabajador
{
    public class CreateTrabajadorCommandHandler : IRequestHandler<CreateTrabajadorCommand, TrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorResponse> Handle(CreateTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Maestros.Trabajadores.Query().Select(t => t.Code), length: 5, entityLabel: "Trabajadores", cancellationToken);

            var trabajador = new Trabajador
            {
                Code = code,
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
                HasDisability = request.HasDisability
            };

            await _uow.Rrhh.Maestros.Trabajadores.AddAsync(trabajador, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(trabajador);
        }

        internal static TrabajadorResponse Map(Trabajador trabajador) => new()
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