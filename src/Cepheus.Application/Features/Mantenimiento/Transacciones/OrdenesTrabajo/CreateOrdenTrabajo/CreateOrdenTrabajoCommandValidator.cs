using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.CreateOrdenTrabajo
{
    public class CreateOrdenTrabajoCommandValidator : AbstractValidator<CreateOrdenTrabajoCommand>
    {
        private static readonly string[] ValidTurnos = { "Manana", "Tarde", "Noche" };

        private readonly IUnitOfWork _uow;

        public CreateOrdenTrabajoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(254).WithMessage("La descripción no puede exceder los 254 caracteres.");

            RuleFor(x => x.FechaProceso)
                .NotEmpty().WithMessage("La fecha de servicio es obligatoria.");

            RuleFor(x => x.ResponsableCode)
                .NotEmpty().WithMessage("El responsable es obligatorio.")
                .MustAsync(ResponsableExists).WithMessage("El Responsable no existe.");
            
            RuleFor(x => x.EspecialidadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La especialidad es obligatoria.")
                .MustAsync(EspecialidadExists).WithMessage("La especialidad indicada no existe.");

            RuleFor(x => x.OportunidadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La oportunidad es obligatoria.")
                .MustAsync(OportunidadExists).WithMessage("La oportunidad indicada no existe.");

            RuleFor(x => x.EquipoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El equipo es obligatorio.")
                .MustAsync(EquipoExists).WithMessage("El equipo indicado no existe.");

            RuleFor(x => x.PrioridadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La prioridad es obligatoria.")
                .MustAsync(PrioridadExists).WithMessage("La prioridad indicada no existe.");

            RuleFor(x => x.InspeccionCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La inspección es obligatoria.")
                .MustAsync(InspeccionExists).WithMessage("La inspección indicada no existe.");

            RuleFor(x => x.TipoOrdenCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de orden es obligatorio.")
                .MustAsync(TipoOrdenExists).WithMessage("El tipo de orden indicado no existe.");

            RuleFor(x => x.ActividadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La actividad es obligatoria.")
                .MustAsync(ActividadExists).WithMessage("La actividad indicada no existe.");

            RuleFor(x => x.SubCentroCostoCode)
                .MustAsync(SubCentroCostoExists).WithMessage("El subcentro de costo indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.SubCentroCostoCode));

            RuleFor(x => x.SubCentroEjecutorCode)
                .MustAsync(SubCentroEjecutorExists).WithMessage("El subcentro ejecutor indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.SubCentroEjecutorCode));

            RuleFor(x => x.UserId)
                .MustAsync(UserExists).WithMessage("El usuario indicado no existe.")
                .When(x => x.UserId.HasValue);

            RuleFor(x => x.Turno)
                .Must(t => ValidTurnos.Contains(t, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"El turno debe ser uno de: {string.Join(", ", ValidTurnos)}.");
        }

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ResponsableExists(string code, CancellationToken ct)
            => await _uow.Rrhh.Maestros.Trabajadores.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);


        private async Task<bool> EspecialidadExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Catalogos.Especialidades.Query().AnyAsync(e => e.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> OportunidadExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Catalogos.Oportunidades.Query().AnyAsync(o => o.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> EquipoExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Maestros.Equipos.Query().AnyAsync(e => e.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> PrioridadExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Catalogos.Prioridades.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> InspeccionExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Catalogos.Inspecciones.Query().AnyAsync(i => i.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> TipoOrdenExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Catalogos.TiposOrden.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ActividadExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Maestros.Actividades.Query().AnyAsync(a => a.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> CentroCostoExists(string? code, CancellationToken ct)
            => await _uow.Logistica.Maestros.CentrosCosto.Query().AnyAsync(c => c.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> SubCentroCostoExists(string? code, CancellationToken ct)
            => await _uow.Logistica.Maestros.SubCentrosCosto.Query().AnyAsync(s => s.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> SubCentroEjecutorExists(string? code, CancellationToken ct)
            => await _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query().AnyAsync(s => s.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> UserExists(int? userId, CancellationToken ct)
            => await _uow.Administracion.Users.Query().AnyAsync(u => u.Id == userId!.Value, ct);
    }
}
