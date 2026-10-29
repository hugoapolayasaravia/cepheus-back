using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.CreateOrdenServicio;

public sealed class CreateOrdenServicioCommandValidator : AbstractValidator<CreateOrdenServicioCommand>
{
    public CreateOrdenServicioCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2).WithMessage("La planta es obligatoria.");
        RuleFor(x => x.ComprobantePagoCode).NotEmpty().MaximumLength(2).WithMessage("El comprobante de pago es obligatorio.");
        RuleFor(x => x.NumeroDocumento).MaximumLength(15).WithMessage("El número de documento no puede exceder 15 caracteres.");
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

        RuleFor(x => x.Detalles)
            .NotEmpty().WithMessage("La Orden de Servicio debe tener al menos una línea de detalle.");

        RuleForEach(x => x.Detalles).SetValidator(new OrdenServicioDetalleRequestValidator());
    }
}
