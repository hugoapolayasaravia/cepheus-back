// Cepheus.Application/Features/Logistica/Transacciones/CotizacionProveedores/InvitarProveedor/InvitarProveedorCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.InvitarProveedor
{
    public class InvitarProveedorCommandValidator : AbstractValidator<InvitarProveedorCommand>
    {
        private readonly IUnitOfWork _uow;

        public InvitarProveedorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.CotizacionCode).NotEmpty();

            RuleFor(x => x.ProveedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El proveedor es obligatorio.")
                .MustAsync(async (code, ct) => await _uow.Logistica.Maestros.Proveedores.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct))
                .WithMessage("El proveedor indicado no existe.");

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .MustAsync(async (code, ct) => await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == code.Trim().ToUpper(), ct))
                .WithMessage("La moneda indicada no existe.");
        }
    }
}