// Cepheus.Application/Features/Logistica/Transacciones/Guias/CreateGuia/CreateGuiaCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.CreateGuia
{
    public class CreateGuiaCommandValidator : AbstractValidator<CreateGuiaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateGuiaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

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
                .MaximumLength(70).WithMessage("El punto de partida no puede exceder los 70 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.PuntoPartida));

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

            RuleFor(x => x.Detalles)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La guía debe tener al menos una línea de detalle.")
                .Must(NoRepeatedArticulos).WithMessage("No se puede repetir un artículo en la misma guía.");

            RuleForEach(x => x.Detalles).ChildRules(line =>
            {
                line.RuleFor(l => l.ArticuloCode)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("El artículo es obligatorio.")
                    .MustAsync(ArticuloExists).WithMessage("El artículo indicado no existe.");

                line.RuleFor(l => l.Cantidad).ValidCantidad();
            });
        }

        private static bool NoRepeatedArticulos(List<CreateGuiaDetalleLineaInput> lines)
            => lines
                .Where(l => !string.IsNullOrWhiteSpace(l.ArticuloCode))
                .Select(l => l.ArticuloCode.Trim().ToUpperInvariant())
                .Distinct()
                .Count() == lines.Count(l => !string.IsNullOrWhiteSpace(l.ArticuloCode));

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

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

        private async Task<bool> VehiculoPerteneceAlTransportista(CreateGuiaCommand cmd, CancellationToken ct)
            => await _uow.Logistica.Maestros.Vehiculos.Query()
                .AnyAsync(v => v.Code == cmd.VehiculoCode.Trim().ToUpper()
                            && v.TransportistaCode == cmd.TransportistaCode.Trim().ToUpper(), ct);

        private async Task<bool> ArticuloExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Articulos.Query().AnyAsync(a => a.Code == code.Trim().ToUpper(), ct);
    }
}
