using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.GenerateNotaCreditoVentaFromSolicitud
{
    public class GenerateNotaCreditoVentaFromSolicitudCommandValidator
        : AbstractValidator<GenerateNotaCreditoVentaFromSolicitudCommand>
    {
        private readonly IUnitOfWork _uow;

        public GenerateNotaCreditoVentaFromSolicitudCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.SolicitudYear).NotEmpty().Length(4).WithMessage("El año de la solicitud debe tener 4 caracteres.");
            RuleFor(x => x.SolicitudMonth).NotEmpty().Length(2).WithMessage("El mes de la solicitud debe tener 2 caracteres.");
            RuleFor(x => x.SolicitudCode).NotEmpty().WithMessage("El código de la solicitud es obligatorio.");

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.ExecutingPlantaCode)
                .MustAsync(PlantaExists).WithMessage("La planta ejecutora indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ExecutingPlantaCode));

            RuleFor(x => x.IssueDate).NotEmpty().WithMessage("La fecha de emisión es obligatoria.");

            RuleFor(x => x.ObraCode)
                .NotEmpty().WithMessage("La obra es obligatoria.")
                .MaximumLength(3).WithMessage("El código de obra debe tener 3 caracteres.");

            RuleFor(x => x.VendedorCode)
                .MustAsync(VendedorExists).WithMessage("El vendedor indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.VendedorCode));

            RuleFor(x => x.FormaPagoVentaCode)
                .MustAsync(FormaPagoExists).WithMessage("La forma de pago indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.FormaPagoVentaCode));

            RuleFor(x => x.DeliveryAddress)
                .MaximumLength(100).WithMessage("La dirección de entrega no puede exceder los 100 caracteres.");

            RuleFor(x => x.DeliveryUbigeoCode)
                .MustAsync(UbigeoExists).WithMessage("El ubigeo de entrega indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.DeliveryUbigeoCode));

            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("La glosa no puede exceder los 200 caracteres.");

            RuleFor(x => x.Type)
                .Must(t => NotaCreditoVentaRules.AllowedTypes.Contains(t!.Trim().ToUpperInvariant()))
                .WithMessage($"El tipo debe ser uno de: {string.Join(", ", NotaCreditoVentaRules.AllowedTypes)}.")
                .When(x => !string.IsNullOrWhiteSpace(x.Type));

            RuleFor(x => x.CashDocument)
                .MaximumLength(10).WithMessage("El documento de caja no puede exceder los 10 caracteres.");
        }

        private async Task<bool> PlantaExists(string? code, CancellationToken cancellationToken)
        {
            var c = NotaCreditoVentaRules.Normalize(code!);
            return await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == c, cancellationToken);
        }

        private async Task<bool> VendedorExists(string? code, CancellationToken cancellationToken)
        {
            var c = code!.Trim();
            return await _uow.Facturacion.Maestros.Vendedores.Query().AnyAsync(v => v.Code == c, cancellationToken);
        }

        private async Task<bool> FormaPagoExists(string? code, CancellationToken cancellationToken)
        {
            var c = code!.Trim();
            return await _uow.Facturacion.Catalogos.FormasPagoVenta.Query().AnyAsync(f => f.Code == c, cancellationToken);
        }

        private async Task<bool> UbigeoExists(string? code, CancellationToken cancellationToken)
        {
            var c = code!.Trim();
            return await _uow.Comunes.Ubigeos.Query().AnyAsync(u => u.Code == c, cancellationToken);
        }
    }
}
