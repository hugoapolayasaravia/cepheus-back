using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common
{
    /// <summary>
    /// Validaciones de forma de la cabecera (legacy ue_set_btn_grb_data). Las reglas que
    /// consultan datos (período, cuenta, forma de pago) se validan en PagoProveedorRules.
    /// </summary>
    public abstract class PagoProveedorCabeceraValidator<T> : AbstractValidator<T> where T : IPagoProveedorCabecera
    {
        protected PagoProveedorCabeceraValidator()
        {
            RuleFor(x => x.ProveedorCode).NotEmpty().WithMessage("El proveedor es obligatorio.")
                .MaximumLength(5).WithMessage("El código del proveedor no puede exceder los 5 caracteres.");

            RuleFor(x => x.FechaEmision)
                .Must(f => f.Year >= 1990).WithMessage("Fecha errada.");

            RuleFor(x => x.BancoCode).NotEmpty().WithMessage("Banco errado.")
                .MaximumLength(3).WithMessage("El código del banco no puede exceder los 3 caracteres.");

            RuleFor(x => x.CuentaBancariaNumero).NotEmpty().WithMessage("La cuenta bancaria es obligatoria.")
                .MaximumLength(20).WithMessage("El número de cuenta no puede exceder los 20 caracteres.");

            RuleFor(x => x.NumeroControl)
                .NotEmpty().WithMessage("Número de control errado.")
                .Length(1).WithMessage("El número de control debe tener 1 carácter.");

            RuleFor(x => x.NumeroCheque)
                .MaximumLength(10).WithMessage("El número de cheque no puede exceder los 10 caracteres.");

            RuleFor(x => x.NumeroCheque)
                .NotEmpty().WithMessage("Número de cheque errado.")
                .When(x => !x.Canje && x.NumeroControl != PagoProveedorRules.SinCheque);

            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.")
                .MaximumLength(2).WithMessage("El código de la planta no puede exceder los 2 caracteres.");

            RuleFor(x => x.PagarA).MaximumLength(150).WithMessage("'Pagar a' no puede exceder los 150 caracteres.");
            RuleFor(x => x.Glosa).MaximumLength(200).WithMessage("La glosa no puede exceder los 200 caracteres.");

            RuleFor(x => x.FormaPagoCancelacionCode).NotEmpty().WithMessage("Seleccione una forma de pago.")
                .Length(1).WithMessage("La forma de pago debe tener 1 carácter.");

            RuleFor(x => x.ProveedorCuentaId)
                .NotNull().WithMessage("Seleccione una cuenta bancaria del proveedor.")
                .When(x => x.FormaPagoCancelacionCode == PagoProveedorRules.FormaPagoCuentaProveedor);
        }
    }
}
