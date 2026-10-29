using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.CreateTrabajador
{
    public class CreateTrabajadorCommandValidator : AbstractValidator<CreateTrabajadorCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.FirstNames)
                .MaximumLength(60).WithMessage("Los nombres no pueden exceder los 60 caracteres.");

            RuleFor(x => x.PaternalSurname)
                .MaximumLength(60).WithMessage("El apellido paterno no puede exceder los 60 caracteres.");

            RuleFor(x => x.MaternalSurname)
                .MaximumLength(60).WithMessage("El apellido materno no puede exceder los 60 caracteres.");

            RuleFor(x => x.SexoCode)
                .MustAsync(SexoExists).WithMessage("El sexo indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.SexoCode));

            RuleFor(x => x.EstadoCivilCode)
                .MustAsync(EstadoCivilExists).WithMessage("El estado civil indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.EstadoCivilCode));

            RuleFor(x => x.NacionalidadCode)
                .MustAsync(NacionalidadExists).WithMessage("La nacionalidad indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.NacionalidadCode));

            RuleFor(x => x.BirthUbigeoCode)
                .MustAsync(UbigeoExists).WithMessage("El ubigeo de nacimiento indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.BirthUbigeoCode));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El correo electrónico no es válido.")
                .MaximumLength(150).WithMessage("El correo electrónico no puede exceder los 150 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(25).WithMessage("El teléfono no puede exceder los 25 caracteres.");

            RuleFor(x => x.MobilePhone)
                .MaximumLength(25).WithMessage("El celular no puede exceder los 25 caracteres.");

            RuleFor(x => x.PhotoUrl)
                .MaximumLength(250).WithMessage("La foto no puede exceder los 250 caracteres.");
        }

        private async Task<bool> SexoExists(string? code, CancellationToken cancellationToken)
            => await _uow.Rrhh.Catalogos.Sexos.Query().AnyAsync(s => s.Code == code!.Trim(), cancellationToken);

        private async Task<bool> EstadoCivilExists(string? code, CancellationToken cancellationToken)
            => await _uow.Rrhh.Catalogos.EstadosCiviles.Query().AnyAsync(e => e.Code == code!.Trim(), cancellationToken);

        private async Task<bool> NacionalidadExists(string? code, CancellationToken cancellationToken)
            => await _uow.Rrhh.Catalogos.Nacionalidades.Query().AnyAsync(n => n.Code == code!.Trim(), cancellationToken);

        private async Task<bool> UbigeoExists(string? code, CancellationToken cancellationToken)
            => await _uow.Comunes.Ubigeos.Query().AnyAsync(u => u.Code == code!.Trim(), cancellationToken);
    }
}