// Cepheus.Application/Features/Logistica/Transacciones/Guias/UpdateGuia/UpdateGuiaCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.UpdateGuia
{
    public class UpdateGuiaCommandValidator : AbstractValidator<UpdateGuiaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateGuiaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.Code).NotEmpty().WithMessage("El número de guía es obligatorio.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("El RowVersion es obligatorio para controlar la concurrencia.");

            RuleFor(x => x.FechaEmision)
                .NotEmpty().WithMessage("La fecha de emisión es obligatoria.");

            RuleFor(x => x.Hora)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La hora es obligatoria.")
                .Matches(GuiaValidationRules.HoraPattern).WithMessage("La hora debe tener el formato HH:mm.");

            RuleFor(x => x.ProveedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El proveedor (destinatario) es obligatorio.")
                .MustAsync(ProveedorExists).WithMessage("El proveedor indicado no existe.");

            RuleFor(x => x.MotivoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El motivo es obligatorio.")
                .MustAsync(MotivoExists).WithMessage("El motivo indicado no existe.");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(70).WithMessage("La dirección no puede exceder los 70 caracteres.");

            RuleFor(x => x.PuntoPartida)
                .NotEmpty().WithMessage("El punto de partida es obligatorio.")
                .MaximumLength(70).WithMessage("El punto de partida no puede exceder los 70 caracteres.");

            RuleFor(x => x.TransportistaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El transportista es obligatorio.")
                .MustAsync(TransportistaExists).WithMessage("El transportista indicado no existe.");

            RuleFor(x => x.ConductorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El conductor es obligatorio.")
                .MustAsync(ConductorExists).WithMessage("El conductor indicado no existe.");

            RuleFor(x => x.VehiculoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El vehículo es obligatorio.")
                .MustAsync(VehiculoExists).WithMessage("El vehículo indicado no existe.");

            RuleFor(x => x)
                .MustAsync(VehiculoPerteneceAlTransportista)
                .WithMessage("El vehículo indicado no pertenece al transportista seleccionado.")
                .When(x => !string.IsNullOrWhiteSpace(x.TransportistaCode)
                        && !string.IsNullOrWhiteSpace(x.VehiculoCode));

            RuleFor(x => x.Observaciones)
                .MaximumLength(50).WithMessage("Las observaciones no pueden exceder los 50 caracteres.");
        }

        private async Task<bool> ProveedorExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Proveedores.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> MotivoExists(string code, CancellationToken ct)
            => await _uow.Comunes.MotivosDevolucion.Query().AnyAsync(m => m.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> TransportistaExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Transportistas.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ConductorExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Conductores.Query().AnyAsync(c => c.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> VehiculoExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Vehiculos.Query().AnyAsync(v => v.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> VehiculoPerteneceAlTransportista(UpdateGuiaCommand cmd, CancellationToken ct)
            => await _uow.Logistica.Maestros.Vehiculos.Query()
                .AnyAsync(v => v.Code == cmd.VehiculoCode.Trim().ToUpper()
                            && v.TransportistaCode == cmd.TransportistaCode.Trim().ToUpper(), ct);
    }
}
