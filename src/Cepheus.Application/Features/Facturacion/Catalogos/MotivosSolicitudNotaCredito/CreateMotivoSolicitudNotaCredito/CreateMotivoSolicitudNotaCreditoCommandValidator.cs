using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.CreateMotivoSolicitudNotaCredito
{
    public class CreateMotivoSolicitudNotaCreditoCommandValidator : AbstractValidator<CreateMotivoSolicitudNotaCreditoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateMotivoSolicitudNotaCreditoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del motivo es obligatorio.")
                .MaximumLength(80).WithMessage("El nombre no puede exceder los 80 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un motivo de solicitud de nota de crédito con ese nombre.");

            RuleFor(x => x.ParentCode)
                .Cascade(CascadeMode.Stop)
                .Length(2).WithMessage("El código del motivo padre debe tener 2 caracteres.")
                .MustAsync(ParentExists).WithMessage("El motivo padre indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ParentCode));

            RuleFor(x => x.MotivoDevolucionCode)
                .Cascade(CascadeMode.Stop)
                .Length(2).WithMessage("El código del motivo de devolución debe tener 2 caracteres.")
                .MustAsync(MotivoDevolucionExists).WithMessage("El motivo de devolución indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.MotivoDevolucionCode));

            RuleFor(x => x.EndDate)
                .Must((cmd, end) => end!.Value.Date >= cmd.StartDate!.Value.Date)
                .WithMessage("La fecha fin de vigencia no puede ser menor a la fecha de inicio.")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query()
                .AnyAsync(t => t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

        private async Task<bool> ParentExists(string? code, CancellationToken cancellationToken)
        {
            var c = code!.Trim();
            return await _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query()
                .AnyAsync(t => t.Code == c, cancellationToken);
        }

        private async Task<bool> MotivoDevolucionExists(string? code, CancellationToken cancellationToken)
        {
            var c = code!.Trim();
            return await _uow.Logistica.Catalogos.MotivosDevolucionArticulo.Query()
                .AnyAsync(m => m.Code == c, cancellationToken);
        }
    }
}
