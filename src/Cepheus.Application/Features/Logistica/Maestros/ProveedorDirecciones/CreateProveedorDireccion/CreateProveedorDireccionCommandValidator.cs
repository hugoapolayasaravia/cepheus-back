using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.CreateProveedorDireccion
{
    public class CreateProveedorDireccionCommandValidator : AbstractValidator<CreateProveedorDireccionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateProveedorDireccionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.ProveedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El proveedor es obligatorio.")
                .Length(5).WithMessage("El código de proveedor debe tener 5 caracteres.")
                .MustAsync(ProveedorExists).WithMessage("El proveedor indicado no existe.");

            RuleFor(x => x.AddressType)
                .IsInEnum().WithMessage("El tipo de dirección no es válido.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(200).WithMessage("La dirección no puede exceder los 200 caracteres.");

            RuleFor(x => x.UbigeoCode)
                .Length(6).WithMessage("El código de ubigeo debe tener 6 caracteres.")
                .MustAsync(UbigeoExists).WithMessage("El ubigeo indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.UbigeoCode));

            RuleFor(x => x.Reference)
                .MaximumLength(200).WithMessage("La referencia no puede exceder los 200 caracteres.");
        }

        private async Task<bool> ProveedorExists(string proveedorCode, CancellationToken cancellationToken)
            => await _uow.Logistica.Maestros.Proveedores.Query()
                .AnyAsync(p => p.Code == proveedorCode.Trim().ToUpper(), cancellationToken);

        private async Task<bool> UbigeoExists(string? ubigeoCode, CancellationToken cancellationToken)
            => await _uow.Comunes.Ubigeos.Query()
                .AnyAsync(u => u.Code == ubigeoCode!.Trim().ToUpper(), cancellationToken);
    }
}
