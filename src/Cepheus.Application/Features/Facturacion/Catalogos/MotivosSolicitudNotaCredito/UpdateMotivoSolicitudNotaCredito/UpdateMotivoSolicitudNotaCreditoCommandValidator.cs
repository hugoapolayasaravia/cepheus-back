using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.UpdateMotivoSolicitudNotaCredito
{
    public class UpdateMotivoSolicitudNotaCreditoCommandValidator : AbstractValidator<UpdateMotivoSolicitudNotaCreditoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateMotivoSolicitudNotaCreditoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del motivo es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del motivo es obligatorio.")
                .MaximumLength(80).WithMessage("El nombre no puede exceder los 80 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro motivo de solicitud de nota de crédito con ese nombre.");

            RuleFor(x => x.ParentCode)
                .Cascade(CascadeMode.Stop)
                .Length(2).WithMessage("El código del motivo padre debe tener 2 caracteres.")
                .Must((cmd, parent) => !string.Equals(parent!.Trim(), cmd.Code.Trim(), StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Un motivo no puede ser su propio padre.")
                .MustAsync(ParentExists).WithMessage("El motivo padre indicado no existe.")
                .MustAsync(NotCircular).WithMessage("El motivo padre indicado genera una referencia circular.")
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

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateMotivoSolicitudNotaCreditoCommand command, string name, CancellationToken cancellationToken)
        {
            var code = command.Code.Trim();
            return !await _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query()
                .AnyAsync(t => t.Code != code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
        }

        private async Task<bool> ParentExists(string? code, CancellationToken cancellationToken)
        {
            var c = code!.Trim();
            return await _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query()
                .AnyAsync(t => t.Code == c, cancellationToken);
        }

        private async Task<bool> NotCircular(UpdateMotivoSolicitudNotaCreditoCommand command, string? parentCode, CancellationToken cancellationToken)
        {
            var self = command.Code.Trim();
            string? current = parentCode!.Trim();
            var visited = new HashSet<string>();

            while (current is not null)
            {
                if (current == self || !visited.Add(current))
                {
                    return false;
                }

                var node = current;
                current = await _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query()
                    .AsNoTracking()
                    .Where(t => t.Code == node)
                    .Select(t => t.ParentCode)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return true;
        }

        private async Task<bool> MotivoDevolucionExists(string? code, CancellationToken cancellationToken)
        {
            var c = code!.Trim();
            return await _uow.Logistica.Catalogos.MotivosDevolucionArticulo.Query()
                .AnyAsync(m => m.Code == c, cancellationToken);
        }
    }
}
