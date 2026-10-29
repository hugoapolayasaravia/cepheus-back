using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.CreateUsuarioSolicitudNotaCredito
{
    public class CreateUsuarioSolicitudNotaCreditoCommandValidator : AbstractValidator<CreateUsuarioSolicitudNotaCreditoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateUsuarioSolicitudNotaCreditoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NegocioCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El negocio es obligatorio.")
                .MustAsync(NegocioExists).WithMessage("El negocio indicado no existe.");

            RuleFor(x => x.Stage)
                .Must(s => EtapaSolicitudNotaCreditoParser.TryParse(s, out _))
                .WithMessage($"La etapa debe ser una de: {EtapaSolicitudNotaCreditoParser.ValidNames}.");

            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("El usuario es obligatorio.")
                .MustAsync(UserExistsAndActive).WithMessage("El usuario indicado no existe o está inactivo.");

            RuleFor(x => x)
                .Must(x => x.CanEnter || x.CanManage || x.CanApprove)
                .WithMessage("Debe habilitar al menos una facultad: ingresar, gestionar o aprobar.")
                .WithName("Facultades");

            RuleFor(x => x)
                .MustAsync(NotAlreadyAssigned)
                .WithMessage("El usuario ya está asignado a esa etapa para el negocio indicado.")
                .WithName("Usuario")
                .When(x => EtapaSolicitudNotaCreditoParser.TryParse(x.Stage, out _) && !string.IsNullOrWhiteSpace(x.NegocioCode));

            RuleFor(x => x)
                .MustAsync(NoOtherActivePrincipal)
                .WithMessage("Ya existe otro usuario principal activo para esa etapa y negocio.")
                .WithName("IsPrincipal")
                .When(x => x.IsPrincipal && EtapaSolicitudNotaCreditoParser.TryParse(x.Stage, out _) && !string.IsNullOrWhiteSpace(x.NegocioCode));

            RuleFor(x => x.EndDate)
                .Must((cmd, end) => end!.Value.Date >= cmd.StartDate!.Value.Date)
                .WithMessage("La fecha fin de vigencia no puede ser menor a la fecha de inicio.")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
        }

        private async Task<bool> NegocioExists(string code, CancellationToken cancellationToken)
        {
            var c = code.Trim().ToUpperInvariant();
            return await _uow.Comunes.Negocios.Query().AnyAsync(n => n.Code == c, cancellationToken);
        }

        private async Task<bool> UserExistsAndActive(int userId, CancellationToken cancellationToken)
            => await _uow.Administracion.Users.Query()
                .AnyAsync(u => u.Id == userId && u.IsActive, cancellationToken);

        private async Task<bool> NotAlreadyAssigned(CreateUsuarioSolicitudNotaCreditoCommand cmd, CancellationToken cancellationToken)
        {
            var negocio = cmd.NegocioCode.Trim().ToUpperInvariant();
            var stage = EtapaSolicitudNotaCreditoParser.Parse(cmd.Stage);
            return !await _uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Query()
                .AnyAsync(u => u.NegocioCode == negocio && u.Stage == stage && u.UserId == cmd.UserId, cancellationToken);
        }

        private async Task<bool> NoOtherActivePrincipal(CreateUsuarioSolicitudNotaCreditoCommand cmd, CancellationToken cancellationToken)
        {
            var negocio = cmd.NegocioCode.Trim().ToUpperInvariant();
            var stage = EtapaSolicitudNotaCreditoParser.Parse(cmd.Stage);
            return !await _uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Query()
                .AnyAsync(u => u.NegocioCode == negocio && u.Stage == stage && u.IsPrincipal && u.IsActive, cancellationToken);
        }
    }
}
