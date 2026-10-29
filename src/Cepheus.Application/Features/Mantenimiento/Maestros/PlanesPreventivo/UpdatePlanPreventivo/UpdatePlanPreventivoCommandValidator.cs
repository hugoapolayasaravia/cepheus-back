using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.UpdatePlanPreventivo
{
    public class UpdatePlanPreventivoCommandValidator : AbstractValidator<UpdatePlanPreventivoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePlanPreventivoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder 100 caracteres.");

            RuleFor(x => x.EquipoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El equipo es obligatorio.")
                .MaximumLength(8).WithMessage("El código de equipo no puede exceder 8 caracteres.")
                .MustAsync(EquipoExists).WithMessage("El equipo no existe.");

            RuleFor(x => x.EspecialidadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La especialidad es obligatoria.")
                .MaximumLength(1).WithMessage("El código de especialidad no puede exceder 1 carácter.")
                .MustAsync(EspecialidadExists).WithMessage("La especialidad no existe.");

            RuleFor(x => x.ResponsableCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El responsable es obligatorio.")
                .MaximumLength(5).WithMessage("El código de trabajador no puede exceder 5 caracteres.")
                .MustAsync(TrabajadorExists).WithMessage("El responsable (trabajador) no existe.");

            RuleFor(x => x.SubCentroCostoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El subcentro de costo es obligatorio.")
                .MaximumLength(6).WithMessage("El código de subcentro de costo no puede exceder 6 caracteres.")
                .MustAsync(SubCentroCostoExists).WithMessage("El subcentro de costo no existe.");

            RuleFor(x => x.SubCentroEjecutorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El subcentro ejecutor es obligatorio.")
                .MaximumLength(4).WithMessage("El código de subcentro ejecutor no puede exceder 4 caracteres.")
                .MustAsync(SubCentroEjecutorExists).WithMessage("El subcentro ejecutor no existe.");

            RuleFor(x => x.ActividadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La actividad es obligatoria.")
                .MaximumLength(6).WithMessage("El código de actividad no puede exceder 6 caracteres.")
                .MustAsync(ActividadExists).WithMessage("La actividad no existe.");

            RuleFor(x => x.Frecuencia).GreaterThanOrEqualTo(0).WithMessage("La frecuencia no puede ser negativa.");
            RuleFor(x => x.UnidadFrecuencia).IsInEnum().WithMessage("La unidad de frecuencia no es válida.");
            RuleFor(x => x.Factor).GreaterThanOrEqualTo(0).WithMessage("El factor no puede ser negativo.");
            RuleFor(x => x.FrecuenciaGama).GreaterThanOrEqualTo(0).WithMessage("La frecuencia de la gama no puede ser negativa.");
            RuleFor(x => x.TiempoRestante).GreaterThanOrEqualTo(0).WithMessage("El tiempo restante no puede ser negativo.");

            RuleFor(x => x.FechaTermino)
                .GreaterThanOrEqualTo(x => x.FechaInicio)
                .WithMessage("La fecha de término no puede ser menor a la de inicio.")
                .When(x => x.FechaInicio.HasValue && x.FechaTermino.HasValue);
        }

        private static string N(string code) => code.Trim().ToUpperInvariant();

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == N(code), ct);
        private async Task<bool> EquipoExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Maestros.Equipos.Query().AnyAsync(e => e.Code == N(code), ct);
        private async Task<bool> EspecialidadExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Catalogos.Especialidades.Query().AnyAsync(e => e.Code == N(code), ct);
        private async Task<bool> TrabajadorExists(string code, CancellationToken ct)
            => await _uow.Rrhh.Maestros.Trabajadores.Query().AnyAsync(t => t.Code == N(code), ct);
        private async Task<bool> SubCentroCostoExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.SubCentrosCosto.Query().AnyAsync(s => s.Code == N(code), ct);
        private async Task<bool> SubCentroEjecutorExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query().AnyAsync(s => s.Code == N(code), ct);
        private async Task<bool> ActividadExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Maestros.Actividades.Query().AnyAsync(a => a.Code == N(code), ct);
    }
}
