using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.UpdateOrdenTrabajo
{
    public class UpdateOrdenTrabajoCommandValidator : AbstractValidator<UpdateOrdenTrabajoCommand>
    {
        private static readonly string[] ValidTurnos = { "Manana", "Tarde", "Noche" };

        private readonly IUnitOfWork _uow;

        public UpdateOrdenTrabajoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(254).WithMessage("La descripción no puede exceder los 254 caracteres.");

            RuleFor(x => x.FechaProceso).NotEmpty().WithMessage("La fecha de proceso es obligatoria.");

            RuleFor(x => x.ResponsableCode).NotEmpty().WithMessage("El responsable es obligatorio.");

            RuleFor(x => x.EspecialidadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La especialidad es obligatoria.")
                .MustAsync(async (code, ct) => await _uow.Mantenimiento.Catalogos.Especialidades.Query().AnyAsync(e => e.Code == code.Trim().ToUpper(), ct))
                .WithMessage("La especialidad indicada no existe.");

            RuleFor(x => x.OportunidadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La oportunidad es obligatoria.")
                .MustAsync(async (code, ct) => await _uow.Mantenimiento.Catalogos.Oportunidades.Query().AnyAsync(o => o.Code == code.Trim().ToUpper(), ct))
                .WithMessage("La oportunidad indicada no existe.");

            RuleFor(x => x.EquipoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El equipo es obligatorio.")
                .MustAsync(async (code, ct) => await _uow.Mantenimiento.Maestros.Equipos.Query().AnyAsync(e => e.Code == code.Trim().ToUpper(), ct))
                .WithMessage("El equipo indicado no existe.");

            RuleFor(x => x.PrioridadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La prioridad es obligatoria.")
                .MustAsync(async (code, ct) => await _uow.Mantenimiento.Catalogos.Prioridades.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct))
                .WithMessage("La prioridad indicada no existe.");

            RuleFor(x => x.InspeccionCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La inspección es obligatoria.")
                .MustAsync(async (code, ct) => await _uow.Mantenimiento.Catalogos.Inspecciones.Query().AnyAsync(i => i.Code == code.Trim().ToUpper(), ct))
                .WithMessage("La inspección indicada no existe.");

            RuleFor(x => x.TipoOrdenCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de orden es obligatorio.")
                .MustAsync(async (code, ct) => await _uow.Mantenimiento.Catalogos.TiposOrden.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct))
                .WithMessage("El tipo de orden indicado no existe.");

            RuleFor(x => x.ActividadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La actividad es obligatoria.")
                .MustAsync(async (code, ct) => await _uow.Mantenimiento.Maestros.Actividades.Query().AnyAsync(a => a.Code == code.Trim().ToUpper(), ct))
                .WithMessage("La actividad indicada no existe.");

            RuleFor(x => x.SubCentroCostoCode)
                .MustAsync(async (code, ct) => await _uow.Logistica.Maestros.SubCentrosCosto.Query().AnyAsync(s => s.Code == code!.Trim().ToUpper(), ct))
                .WithMessage("El subcentro de costo indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.SubCentroCostoCode));

            RuleFor(x => x.SubCentroEjecutorCode)
                .MustAsync(async (code, ct) => await _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query().AnyAsync(s => s.Code == code!.Trim().ToUpper(), ct))
                .WithMessage("El subcentro ejecutor indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.SubCentroEjecutorCode));

            RuleFor(x => x.DowntimeHours)
                .GreaterThanOrEqualTo(0).WithMessage("El tiempo de falla no puede ser negativo.");

            RuleFor(x => x.Turno)
                .Must(t => ValidTurnos.Contains(t, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"El turno debe ser uno de: {string.Join(", ", ValidTurnos)}.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
