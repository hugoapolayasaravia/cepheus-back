// Cepheus.Application/Features/Logistica/Transacciones/Guias/GuiaDetalles/CreateGuiaDetalle/CreateGuiaDetalleCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.CreateGuiaDetalle
{
    public class CreateGuiaDetalleCommandValidator : AbstractValidator<CreateGuiaDetalleCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateGuiaDetalleCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.GuiaCode).NotEmpty().WithMessage("El número de guía es obligatorio.");

            RuleFor(x => x.ArticuloCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El artículo es obligatorio.")
                .MustAsync(ArticuloExists).WithMessage("El artículo indicado no existe.");

            RuleFor(x => x.Cantidad).ValidCantidad();
        }

        private async Task<bool> ArticuloExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Articulos.Query().AnyAsync(a => a.Code == code.Trim().ToUpper(), ct);
    }
}
