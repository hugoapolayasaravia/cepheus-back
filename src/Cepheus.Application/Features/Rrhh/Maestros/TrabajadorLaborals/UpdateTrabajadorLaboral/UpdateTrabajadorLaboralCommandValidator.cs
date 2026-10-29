using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.UpdateTrabajadorLaboral
{
    public class UpdateTrabajadorLaboralCommandValidator
        : AbstractValidator<UpdateTrabajadorLaboralCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorLaboralCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            // ============================================================
            // CONTROL DE CONCURRENCIA
            // ============================================================

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion es obligatorio para control de concurrencia.");

            // ============================================================
            // TRABAJADOR
            // ============================================================

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El trabajador es obligatorio.")
                .Length(5)
                .WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists)
                .WithMessage("El trabajador indicado no existe.")
                .MustAsync(BeUniqueTrabajador)
                .WithMessage("El trabajador ya tiene información laboral registrada.");

            // ============================================================
            // TIPO DE TRABAJADOR
            // ============================================================

            RuleFor(x => x.TipoTrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(TipoTrabajadorExists)
                .WithMessage("El tipo de trabajador indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.TipoTrabajadorCode));

            // ============================================================
            // CATEGORÍA DE TRABAJADOR
            // ============================================================

            RuleFor(x => x.CategoriaTrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(CategoriaTrabajadorExists)
                .WithMessage("La categoría de trabajador indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.CategoriaTrabajadorCode));

            // ============================================================
            // ESTADO DE TRABAJADOR
            // ============================================================

            RuleFor(x => x.EstadoTrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(EstadoTrabajadorExists)
                .WithMessage("El estado de trabajador indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.EstadoTrabajadorCode));

            // ============================================================
            // ÁREA
            // ============================================================

            RuleFor(x => x.AreaCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(AreaExists)
                .WithMessage("El área indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.AreaCode));

            // ============================================================
            // OCUPACIÓN
            // ============================================================

            RuleFor(x => x.OcupacionCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(OcupacionExists)
                .WithMessage("La ocupación indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.OcupacionCode));

            // ============================================================
            // SUBOCUPACIÓN
            // ============================================================

            RuleFor(x => x.SubOcupacionCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(SubOcupacionExists)
                .WithMessage("La subocupación indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.SubOcupacionCode));

            // ============================================================
            // OFICINA
            // ============================================================

            RuleFor(x => x.OficinaCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(OficinaExists)
                .WithMessage("La oficina indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.OficinaCode));

            // ============================================================
            // PLANTA
            // ============================================================

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(PlantaExists)
                .WithMessage("La planta indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.PlantaCode));

            // ============================================================
            // CARGO
            // ============================================================

            RuleFor(x => x.CargoCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(CargoExists)
                .WithMessage("El cargo indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.CargoCode));

            // ============================================================
            // NIVEL
            // ============================================================

            RuleFor(x => x.NivelCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(NivelExists)
                .WithMessage("El nivel indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.NivelCode));

            // ============================================================
            // RÉGIMEN LABORAL
            // ============================================================

            RuleFor(x => x.RegimenLaboralCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(RegimenLaboralExists)
                .WithMessage("El régimen laboral indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.RegimenLaboralCode));

            // ============================================================
            // PROVEEDOR
            // ============================================================

            RuleFor(x => x.ProveedorCode)
                .Cascade(CascadeMode.Stop)
                .MustAsync(ProveedorExists)
                .WithMessage("El proveedor indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ProveedorCode));

            // ============================================================
            // FECHAS
            // ============================================================

            RuleFor(x => x.FechaCese)
                .GreaterThanOrEqualTo(x => x.FechaIngreso)
                .WithMessage("La fecha de cese no puede ser anterior a la fecha de ingreso.")
                .When(x => x.FechaIngreso.HasValue && x.FechaCese.HasValue);
        }

        // ================================================================
        // TRABAJADOR
        // ================================================================

        private async Task<bool> TrabajadorExists(
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return await _uow.Rrhh.Maestros.Trabajadores.Query()
                .AnyAsync(
                    x => x.Code == trabajadorCode.Trim(),
                    cancellationToken);
        }

        private async Task<bool> BeUniqueTrabajador(
            UpdateTrabajadorLaboralCommand command,
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorLaborals.Query()
                .AnyAsync(
                    x => x.TrabajadorCode == trabajadorCode.Trim()
                         && x.Id != command.Id,
                    cancellationToken);
        }

        // ================================================================
        // CATÁLOGOS DE RRHH
        // ================================================================

        private async Task<bool> TipoTrabajadorExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.TiposTrabajador.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> CategoriaTrabajadorExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.CategoriasTrabajador.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> EstadoTrabajadorExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.EstadosTrabajador.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> AreaExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.Areas.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> OcupacionExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.Ocupaciones.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> SubOcupacionExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.SubOcupaciones.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> OficinaExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.Oficinas.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> PlantaExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Comunes.Plantas.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> CargoExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.Cargos.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> NivelExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.NivelesTrabajador.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> RegimenLaboralExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.RegimenesLaborales.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        // ================================================================
        // LOGÍSTICA - PROVEEDOR
        // ================================================================

        private async Task<bool> ProveedorExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Logistica.Maestros.Proveedores.Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }
    }
}
