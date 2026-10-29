using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.UpdateUsuarioSolicitudNotaCredito
{
    public class UpdateUsuarioSolicitudNotaCreditoCommandValidator : AbstractValidator<UpdateUsuarioSolicitudNotaCreditoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUsuarioSolicitudNotaCreditoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NegocioCode)
                .NotEmpty().WithMessage("El negocio es obligatorio.")
                .Length(2).WithMessage("El código de negocio debe tener 2 caracteres.");

            RuleFor(x => x.Stage)
                .Must(s => EtapaSolicitudNotaCreditoParser.TryParse(s, out _))
                .WithMessage($"La etapa debe ser una de: {EtapaSolicitudNotaCreditoParser.ValidNames}.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("El usuario es obligatorio.");

            RuleFor(x => x)
                .Must(x => x.CanEnter || x.CanManage || x.CanApprove)
                .WithMessage("Debe habilitar al menos una facultad: ingresar, gestionar o aprobar.")
                .WithName("Facultades");

            RuleFor(x => x)
                .MustAsync(NoOtherActivePrincipal)
                .WithMessage("Ya existe otro usuario principal activo para esa etapa y negocio.")
                .WithName("IsPrincipal")
                .When(x => x.IsPrincipal && EtapaSolicitudNotaCreditoParser.TryParse(x.Stage, out _) && !string.IsNullOrWhiteSpace(x.NegocioCode));

            RuleFor(x => x.EndDate)
                .Must((cmd, end) => end!.Value.Date >= cmd.StartDate!.Value.Date)
                .WithMessage("La fecha fin de vigencia no puede ser menor a la fecha de inicio.")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> NoOtherActivePrincipal(UpdateUsuarioSolicitudNotaCreditoCommand cmd, CancellationToken cancellationToken)
        {
            var negocio = cmd.NegocioCode.Trim().ToUpperInvariant();
            var stage = EtapaSolicitudNotaCreditoParser.Parse(cmd.Stage);
            return !await _uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Query()
                .AnyAsync(u => u.NegocioCode == negocio && u.Stage == stage && u.UserId != cmd.UserId
                               && u.IsPrincipal && u.IsActive, cancellationToken);
        }
    }
}
