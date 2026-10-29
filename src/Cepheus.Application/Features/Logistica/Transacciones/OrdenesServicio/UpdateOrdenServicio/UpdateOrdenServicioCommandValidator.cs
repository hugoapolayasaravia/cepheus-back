using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.UpdateOrdenServicio;

public sealed class UpdateOrdenServicioCommandValidator : AbstractValidator<UpdateOrdenServicioCommand>
{
    public UpdateOrdenServicioCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
        RuleFor(x => x.ComprobantePagoCode).NotEmpty().MaximumLength(2).WithMessage("El comprobante de pago es obligatorio.");
        RuleFor(x => x.NumeroDocumento).MaximumLength(15);
        RuleFor(x => x.ProveedorCode).NotEmpty().MaximumLength(5).WithMessage("El proveedor es obligatorio.");
        RuleFor(x => x.MonedaCode).NotEmpty().MaximumLength(3).WithMessage("La moneda es obligatoria.");
        RuleFor(x => x.FormaPagoCode).NotEmpty().MaximumLength(2).WithMessage("La forma de pago es obligatoria.");
        RuleFor(x => x.FechaEmision).NotEmpty().WithMessage("La fecha de emisión es obligatoria.");
        RuleFor(x => x.FechaRecepcion).NotEmpty().WithMessage("La fecha de recepción es obligatoria.");
        RuleFor(x => x.CompradorCode).NotEmpty().MaximumLength(3).WithMessage("El comprador es obligatorio.");
        RuleFor(x => x.LugarEnvioCode).NotEmpty().MaximumLength(3).WithMessage("El lugar de envío es obligatorio.");
        RuleFor(x => x.TramiteCode).NotEmpty().MaximumLength(1).WithMessage("El trámite (prioridad) es obligatorio.");
        RuleFor(x => x.NotaCompraCode).MaximumLength(3);
        RuleFor(x => x.UnidadNegocioCode).NotEmpty().MaximumLength(6).WithMessage("La unidad de negocio es obligatoria.");
        RuleFor(x => x.TrabajadorCode).NotEmpty().MaximumLength(5).WithMessage("El trabajador responsable es obligatorio.");
        RuleFor(x => x.Observaciones1).MaximumLength(200);
        RuleFor(x => x.Observaciones2).MaximumLength(200);

        RuleFor(x => x.Igv).GreaterThanOrEqualTo(0).WithMessage("El IGV no puede ser negativo.");
        RuleFor(x => x.NoGravable).GreaterThanOrEqualTo(0).WithMessage("El importe no gravable no puede ser negativo.");
        RuleFor(x => x.Renta).GreaterThanOrEqualTo(0).WithMessage("La renta no puede ser negativa.");
        RuleFor(x => x.Fonavi).GreaterThanOrEqualTo(0).WithMessage("El fonavi no puede ser negativo.");
        RuleFor(x => x.Servicio).GreaterThanOrEqualTo(0).WithMessage("El importe de servicio no puede ser negativo.");
        RuleFor(x => x.IgvExterior).GreaterThanOrEqualTo(0).WithMessage("El IGV exterior no puede ser negativo.");
    }
}
