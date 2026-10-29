using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.CreateTrabajadorDomicilio
{
    public class CreateTrabajadorDomicilioCommandValidator : AbstractValidator<CreateTrabajadorDomicilioCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorDomicilioCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El trabajador es obligatorio.")
                .Length(5).WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists).WithMessage("El trabajador indicado no existe.");

            RuleFor(x => x.RoadTypeCode)
                .MustAsync(RoadTypeExists).WithMessage("El tipo de vía indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.RoadTypeCode));

            RuleFor(x => x.StreetName)
                .MaximumLength(150).WithMessage("El nombre de la vía no puede exceder los 150 caracteres.");

            RuleFor(x => x.StreetNumber)
                .MaximumLength(20).WithMessage("El número de vía no puede exceder los 20 caracteres.");

            RuleFor(x => x.InteriorNumber)
                .MaximumLength(20).WithMessage("El interior no puede exceder los 20 caracteres.");

            RuleFor(x => x.ZoneTypeCode)
                .MustAsync(ZoneTypeExists).WithMessage("El tipo de zona indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ZoneTypeCode));

            RuleFor(x => x.ZoneName)
                .MaximumLength(150).WithMessage("El nombre de la zona no puede exceder los 150 caracteres.");

            RuleFor(x => x.Reference)
                .MaximumLength(250).WithMessage("La referencia no puede exceder los 250 caracteres.");

            RuleFor(x => x.UbigeoCode)
                .MustAsync(UbigeoExists).WithMessage("El ubigeo indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.UbigeoCode));
        }

        private async Task<bool> TrabajadorExists(string trabajadorCode, CancellationToken cancellationToken)
            => await _uow.Rrhh.Maestros.Trabajadores.Query()
                .AnyAsync(t => t.Code == trabajadorCode.Trim().ToUpper(), cancellationToken);

        private async Task<bool> RoadTypeExists(string? code, CancellationToken cancellationToken)
            => await _uow.Rrhh.Catalogos.TiposVia.Query().AnyAsync(t => t.Code == code!.Trim().ToUpper(), cancellationToken);

        private async Task<bool> ZoneTypeExists(string? code, CancellationToken cancellationToken)
            => await _uow.Rrhh.Catalogos.TiposZona.Query().AnyAsync(t => t.Code == code!.Trim().ToUpper(), cancellationToken);

        private async Task<bool> UbigeoExists(string? code, CancellationToken cancellationToken)
            => await _uow.Comunes.Ubigeos.Query().AnyAsync(u => u.Code == code!.Trim().ToUpper(), cancellationToken);
    }
}