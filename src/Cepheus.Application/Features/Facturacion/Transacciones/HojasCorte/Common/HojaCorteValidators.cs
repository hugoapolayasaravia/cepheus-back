using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;

/// <summary>Reglas de formato de la cabecera, compartidas por Create y Update.</summary>
public abstract class HojaCorteHeaderValidator<T> : AbstractValidator<T> where T : HojaCorteHeaderRequest
{
    protected HojaCorteHeaderValidator()
    {
        RuleFor(x => x.TipoHojaCorteCode).NotEmpty().MaximumLength(2)
            .WithMessage("El tipo de Hoja de Corte es obligatorio.");

        RuleFor(x => x.ClienteCode).MaximumLength(5);
        RuleFor(x => x.ClientName).MaximumLength(100);
        RuleFor(x => x.ObraCode).MaximumLength(3);
        RuleFor(x => x.WorkName).MaximumLength(100);
        RuleFor(x => x.WorkAddress).MaximumLength(100);
        RuleFor(x => x.WorkAddressUbigeoCode).MaximumLength(6);

        RuleFor(x => x.ObraCode).Empty()
            .When(x => string.IsNullOrWhiteSpace(x.ClienteCode))
            .WithMessage("Para indicar una obra debe indicar el cliente.");

        RuleFor(x => x.ClientName).NotEmpty()
            .When(x => string.IsNullOrWhiteSpace(x.ClienteCode))
            .WithMessage("Sin cliente registrado debe indicar el nombre del cliente.");

        RuleFor(x => x.WorkAddressUbigeoCode).NotEmpty()
            .When(x => string.IsNullOrWhiteSpace(x.ObraCode))
            .WithMessage("Sin obra registrada debe indicar el ubigeo de la dirección.");

        RuleFor(x => x.TecnicoCode).NotEmpty().MaximumLength(5)
            .WithMessage("El técnico es obligatorio.");

        RuleFor(x => x.VendedorCode).MaximumLength(4);
        RuleFor(x => x.Sector).MaximumLength(50);

        RuleFor(x => x.Floor).NotEmpty().MaximumLength(50)
            .WithMessage("El piso o nivel es obligatorio.");

        RuleFor(x => x.ClientViguetaDispatchDate).NotEmpty()
            .WithMessage("La fecha posible de despacho de viguetas es obligatoria.");
        RuleFor(x => x.ClientBovedillaDispatchDate).NotEmpty()
            .WithMessage("La fecha posible de despacho de bovedillas es obligatoria.");

        RuleFor(x => x.Observations).MaximumLength(4000);

        RuleFor(x => x.Turn).IsInEnum().WithMessage("El turno no es válido (Manana, Tarde, Noche).");

        RuleFor(x => x.Motivo).IsInEnum().When(x => x.Motivo.HasValue)
            .WithMessage("El motivo no es válido.");

        RuleFor(x => x.ProductType).NotNull().IsInEnum()
            .WithMessage("El tipo de producto es obligatorio (Vigueta, Bovedilla, ViguetaYBovedilla).");
    }
}

/// <summary>Formato de los campos de línea (alta y modificación).</summary>
public class HojaCorteDetalleFieldsValidator<T> : AbstractValidator<T> where T : HojaCorteDetalleFields
{
    public HojaCorteDetalleFieldsValidator()
    {
        RuleFor(x => x.UnitCode).MaximumLength(2);
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");
        RuleFor(x => x.Panel).MaximumLength(5);
        RuleFor(x => x.Length).GreaterThanOrEqualTo(0).When(x => x.Length.HasValue);
        RuleFor(x => x.CablesSide1).GreaterThanOrEqualTo(0).When(x => x.CablesSide1.HasValue);
        RuleFor(x => x.CablesSide2).GreaterThanOrEqualTo(0).When(x => x.CablesSide2.HasValue);
        RuleFor(x => x.Lot1).MaximumLength(5);
        RuleFor(x => x.Lot2).MaximumLength(5);
        RuleFor(x => x.Lot3).MaximumLength(5);
    }
}

public sealed class HojaCorteDetalleRequestValidator : HojaCorteDetalleFieldsValidator<HojaCorteDetalleRequest>
{
    public HojaCorteDetalleRequestValidator()
    {
        RuleFor(x => x.ProductoTipoCode).NotEmpty()
            .Must(t => HojaCorteProductoTipos.IsValid(t?.Trim() ?? string.Empty))
            .WithMessage("ProductoTipoCode debe ser 16 (vigueta) o 18 (bovedilla).");

        RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4)
            .WithMessage("El código de producto es obligatorio.");
    }
}

public sealed class UpdateHojaCorteDetalleRequestValidator : HojaCorteDetalleFieldsValidator<UpdateHojaCorteDetalleRequest>
{
}
