using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.CreateNotaIngreso;

public sealed class CreateNotaIngresoCommandValidator : AbstractValidator<CreateNotaIngresoCommand>
{
    public CreateNotaIngresoCommandValidator(IUnitOfWork uow)
    {
        RuleFor(x => x.PlantaCode).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("La planta es obligatoria.")
            .MaximumLength(2)
            .MustAsync(async (c, ct) => await uow.Comunes.Plantas.Query()
                .AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
            .WithMessage("La planta indicada no existe.");

        RuleFor(x => x.Condicion)
            .IsInEnum()
            .Equal(CondicionNotaIngreso.OrdenCompra)
            .WithMessage("La condición 'Anexar Guías' aún no está disponible.");

        RuleFor(x => x.Origen)
            .IsInEnum()
            .Equal(OrigenNotaIngreso.Compra)
            .WithMessage("Por ahora solo se admite el origen Compra.");

        RuleFor(x => x.ComprobantePagoCode).NotEmpty()
            .WithMessage("El comprobante de pago es obligatorio.");

        RuleFor(x => x.FechaEmision).NotEmpty().WithMessage("La fecha de emisión es obligatoria.");
        RuleFor(x => x.FechaRecepcion).NotEmpty().WithMessage("La fecha de recepción es obligatoria.");

        RuleFor(x => x.OrdenCompraCode).MaximumLength(6);
        RuleFor(x => x.NumeroDocumento).MaximumLength(15);
        RuleFor(x => x.NumeroGuia).MaximumLength(15);
        RuleFor(x => x.NumeroReferencia).MaximumLength(15);
        RuleFor(x => x.ProveedorCode).MaximumLength(5);

        RuleFor(x => x.Detalles).NotEmpty().WithMessage("Debe indicar al menos un artículo.");

        RuleForEach(x => x.Detalles).ChildRules(d =>
        {
            d.RuleFor(l => l.ArticuloCode).NotEmpty().MaximumLength(7)
                .WithMessage("El código de artículo es obligatorio.");
            d.RuleFor(l => l.PedidoCode).MaximumLength(6);
            d.RuleFor(l => l.Cantidad).GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a cero.");
            d.RuleFor(l => l.Precio).GreaterThanOrEqualTo(0)
                .WithMessage("El precio no puede ser negativo.");
            d.RuleFor(l => l.Descuento).InclusiveBetween(0, 100)
                .WithMessage("El descuento es un porcentaje entre 0 y 100.");
        });
    }
}
