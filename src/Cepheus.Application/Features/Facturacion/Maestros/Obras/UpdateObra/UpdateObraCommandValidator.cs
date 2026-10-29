using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.UpdateObra
{
    public class UpdateObraCommandValidator : AbstractValidator<UpdateObraCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateObraCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.ClienteCode)
                .NotEmpty().WithMessage("El cliente es obligatorio.")
                .Length(5).WithMessage("El código de cliente debe tener 5 caracteres.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la obra es obligatorio.")
                .Length(3).WithMessage("El código de la obra debe tener 3 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(80).WithMessage("La descripción no puede exceder los 80 caracteres.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(100).WithMessage("La dirección no puede exceder los 100 caracteres.");

            RuleFor(x => x.UbigeoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El ubigeo es obligatorio.")
                .Length(6).WithMessage("El código de ubigeo debe tener 6 caracteres.")
                .MustAsync(UbigeoExists).WithMessage("El ubigeo indicado no existe.");

            RuleFor(x => x.Observations).MaximumLength(150);

            RuleFor(x => x.DeliveryAddress).MaximumLength(100);

            RuleFor(x => x.DeliveryUbigeoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El ubigeo de entrega es obligatorio.")
                .Length(6).WithMessage("El código de ubigeo de entrega debe tener 6 caracteres.")
                .MustAsync(UbigeoExists).WithMessage("El ubigeo de entrega indicado no existe.");

            RuleFor(x => x.BillingAddress)
                .NotEmpty().WithMessage("La dirección de cobranza es obligatoria.")
                .MaximumLength(100).WithMessage("La dirección de cobranza no puede exceder los 100 caracteres.");

            RuleFor(x => x.BillingUbigeoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El ubigeo de cobranza es obligatorio.")
                .Length(6).WithMessage("El código de ubigeo de cobranza debe tener 6 caracteres.")
                .MustAsync(UbigeoExists).WithMessage("El ubigeo de cobranza indicado no existe.");

            RuleFor(x => x.ResponsibleName).MaximumLength(50);

            RuleFor(x => x.ResponsiblePhone)
                .NotEmpty().WithMessage("El teléfono del responsable es obligatorio.")
                .MaximumLength(50).WithMessage("El teléfono del responsable no puede exceder los 50 caracteres.");

            RuleFor(x => x.ResponsibleEmail)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El correo del responsable es obligatorio.")
                .MaximumLength(50).WithMessage("El correo del responsable no puede exceder los 50 caracteres.")
                .EmailAddress().WithMessage("El correo del responsable no tiene un formato válido.");

            RuleFor(x => x.FormaPagoVentaCode)
                .Cascade(CascadeMode.Stop)
                .Length(2).WithMessage("El código de forma de pago de ventas debe tener 2 caracteres.")
                .MustAsync(FormaPagoVentaExists).WithMessage("La forma de pago de ventas indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.FormaPagoVentaCode));

            RuleFor(x => x.CobradorCode)
                .NotEmpty().WithMessage("El cobrador es obligatorio.")
                .MaximumLength(4).WithMessage("El código de cobrador no puede exceder los 4 caracteres.");

            RuleFor(x => x.VendedorCode)
                .NotEmpty().WithMessage("El vendedor es obligatorio.")
                .MaximumLength(4).WithMessage("El código de vendedor no puede exceder los 4 caracteres.");

            RuleFor(x => x.AnalisisVentaCode)
                .Cascade(CascadeMode.Stop)
                .Length(3).WithMessage("El código de análisis de ventas debe tener 3 caracteres.")
                .MustAsync(AnalisisVentaExists).WithMessage("El análisis de ventas indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.AnalisisVentaCode));

            RuleFor(x => x.CreditLimit)
                .GreaterThanOrEqualTo(0).WithMessage("La línea de crédito no puede ser negativa.");

            RuleFor(x => x.CreditCurrencyCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda de la línea de crédito es obligatoria.")
                .MaximumLength(3).WithMessage("El código de moneda no puede exceder los 3 caracteres.")
                .MustAsync(CurrencyExists).WithMessage("La moneda indicada no existe.");

            RuleFor(x => x.ShortName)
                .NotEmpty().WithMessage("El nombre corto es obligatorio.")
                .MaximumLength(10).WithMessage("El nombre corto no puede exceder los 10 caracteres.");

            RuleFor(x => x.TipoValorizacionCode)
                .Cascade(CascadeMode.Stop)
                .Length(1).WithMessage("El código de tipo de valorización debe tener 1 carácter.")
                .MustAsync(TipoValorizacionExists).WithMessage("El tipo de valorización indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.TipoValorizacionCode));

            RuleFor(x => x.ScheduledWeekday)
                .Must(BeValidDiaSemana).WithMessage("El día de la semana indicado no es válido.")
                .When(x => !string.IsNullOrWhiteSpace(x.ScheduledWeekday));

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private static bool BeValidDiaSemana(string? value)
            => value is not null && System.Enum.TryParse<DiaSemana>(value, true, out _);

        private async Task<bool> UbigeoExists(string code, CancellationToken ct)
            => await _uow.Comunes.Ubigeos.Query().AnyAsync(u => u.Code == code.Trim(), ct);

        private async Task<bool> FormaPagoVentaExists(string? code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.FormasPagoVenta.Query().AnyAsync(f => f.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> AnalisisVentaExists(string? code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.AnalisisVentas.Query().AnyAsync(a => a.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> CurrencyExists(string code, CancellationToken ct)
            => await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> TipoValorizacionExists(string? code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.TiposValorizacion.Query().AnyAsync(t => t.Code == code!.Trim().ToUpper(), ct);
    }
}
