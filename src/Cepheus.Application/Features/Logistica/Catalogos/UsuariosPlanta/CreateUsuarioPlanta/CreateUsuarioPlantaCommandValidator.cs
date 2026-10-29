using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.CreateUsuarioPlanta
{
    public class CreateUsuarioPlantaCommandValidator : AbstractValidator<CreateUsuarioPlantaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateUsuarioPlantaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("El usuario es obligatorio.")
                .MustAsync(UserExists).WithMessage("El usuario no existe.")
                .When(x => x.UserId > 0);

            RuleFor(x => x.PlantaCode)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MaximumLength(2).WithMessage("El código de planta no puede exceder 2 caracteres.")
                .MustAsync(PlantaExists).WithMessage("La planta no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.PlantaCode));

            RuleFor(x => x)
                .MustAsync(BeUniqueAssignment)
                .WithMessage("El usuario ya está asignado a esa planta.")
                .OverridePropertyName(nameof(CreateUsuarioPlantaCommand.PlantaCode))
                .When(x => x.UserId > 0 && !string.IsNullOrWhiteSpace(x.PlantaCode));
        }

        private async Task<bool> UserExists(int userId, CancellationToken ct)
            => await _uow.Administracion.Users.Query().AnyAsync(u => u.Id == userId, ct);

        private async Task<bool> PlantaExists(string plantaCode, CancellationToken ct)
        {
            var code = plantaCode.Trim().ToUpperInvariant();
            return await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code, ct);
        }

        private async Task<bool> BeUniqueAssignment(CreateUsuarioPlantaCommand command, CancellationToken ct)
        {
            var code = command.PlantaCode.Trim().ToUpperInvariant();
            return !await _uow.Logistica.Catalogos.UsuariosPlanta.Query()
                .AnyAsync(a => a.UserId == command.UserId && a.PlantaCode == code, ct);
        }
    }
}
