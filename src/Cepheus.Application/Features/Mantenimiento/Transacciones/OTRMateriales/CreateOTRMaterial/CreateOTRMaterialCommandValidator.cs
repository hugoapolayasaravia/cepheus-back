using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.CreateOTRMaterial
{
    public class CreateOTRMaterialCommandValidator : AbstractValidator<CreateOTRMaterialCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateOTRMaterialCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.OrdenTrabajoCode).NotEmpty().WithMessage("La orden de trabajo es obligatoria.");

            RuleFor(x => x.ArticuloCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El artículo es obligatorio.")
                .MustAsync(ArticuloExists).WithMessage("El artículo indicado no existe.");

            RuleFor(x => x)
                .MustAsync(OrdenTrabajoExists)
                .WithMessage("La orden de trabajo indicada no existe.")
                .OverridePropertyName(nameof(CreateOTRMaterialCommand.OrdenTrabajoCode));

            RuleFor(x => x.Cantidad).GreaterThanOrEqualTo(0).WithMessage("La cantidad no puede ser negativa.");
            RuleFor(x => x.CostoUnitario).GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");
            RuleFor(x => x.CostoTotal).GreaterThanOrEqualTo(0).WithMessage("El costo total no puede ser negativo.");

            RuleFor(x => x.EstadoCode)
                .MaximumLength(2).WithMessage("El código de estado no puede exceder los 2 caracteres.");
        }

        private async Task<bool> ArticuloExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Articulos.Query().AnyAsync(a => a.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> OrdenTrabajoExists(CreateOTRMaterialCommand command, CancellationToken ct)
        {
            var plantaCode = command.PlantaCode.Trim().ToUpper();
            var ordenCode = command.OrdenTrabajoCode.Trim().ToUpper();

            return await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query()
                .AnyAsync(o => o.PlantaCode == plantaCode && o.Code == ordenCode, ct);
        }
    }
}
