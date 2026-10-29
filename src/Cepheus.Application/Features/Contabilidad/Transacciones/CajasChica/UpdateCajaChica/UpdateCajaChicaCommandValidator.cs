using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.UpdateCajaChica
{
    public class UpdateCajaChicaCommandValidator : AbstractValidator<UpdateCajaChicaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCajaChicaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la caja es obligatorio.")
                .Length(CajaChicaRules.CodeLength).WithMessage($"El código debe tener {CajaChicaRules.CodeLength} caracteres.");

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El responsable de la caja es obligatorio.")
                .MustAsync(TrabajadorExists).WithMessage("El trabajador indicado no existe.");

            RuleFor(x => x.IssueDate).NotEmpty().WithMessage("La fecha de emisión es obligatoria.");

            RuleFor(x => x.Observations)
                .MaximumLength(200).WithMessage("Las observaciones no pueden exceder los 200 caracteres.");

            RuleFor(x => x.Type)
                .Must(t => CajaChicaRules.TryParseTipo(t, out _))
                .WithMessage($"El tipo debe ser uno de: {CajaChicaRules.TipoNames} (o F, G, R).");

            RuleFor(x => x.Details!)
                .Must(d => d.Count <= CajaChicaRules.MaxLines)
                .WithMessage($"La caja no puede exceder las {CajaChicaRules.MaxLines} líneas.")
                .WithName("Details")
                .When(x => x.Details is not null);

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> PlantaExists(string code, CancellationToken cancellationToken)
        {
            var c = CajaChicaRules.Normalize(code);
            return await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == c, cancellationToken);
        }

        private async Task<bool> TrabajadorExists(string code, CancellationToken cancellationToken)
        {
            var c = code.Trim();
            return await _uow.Rrhh.Maestros.Trabajadores.Query().AnyAsync(t => t.Code == c, cancellationToken);
        }
    }
}
