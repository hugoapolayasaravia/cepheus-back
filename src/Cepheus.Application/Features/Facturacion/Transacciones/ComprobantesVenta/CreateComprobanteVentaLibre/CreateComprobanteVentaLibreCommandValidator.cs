using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.CreateComprobanteVentaLibre;

public sealed class ComprobanteVentaLineaLibreRequestValidator : AbstractValidator<ComprobanteVentaLineaLibreRequest>
{
    public ComprobanteVentaLineaLibreRequestValidator()
    {
        RuleFor(x => x.ProductoTipoCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.Quantity).GreaterThan(0m);
        RuleFor(x => x.UnitPrice).GreaterThan(0m);
    }
}

public sealed class CreateComprobanteVentaLibreCommandValidator : AbstractValidator<CreateComprobanteVentaLibreCommand>
{
    public CreateComprobanteVentaLibreCommandValidator()
    {
        RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Number).MaximumLength(10);
        RuleFor(x => x.PlantCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.ExecutingPlantCode).MaximumLength(2);
        RuleFor(x => x.ClienteCode).NotEmpty().MaximumLength(5);
        RuleFor(x => x.ObraCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.VendedorCode).MaximumLength(4);
        RuleFor(x => x.CurrencyCode).MaximumLength(3);
        RuleFor(x => x.Address).MaximumLength(100);
        RuleFor(x => x.UbigeoCode).MaximumLength(6);
        RuleFor(x => x.Glosa).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Detalles).NotEmpty().WithMessage("El comprobante debe tener al menos una línea.");
        RuleForEach(x => x.Detalles).SetValidator(new ComprobanteVentaLineaLibreRequestValidator());
    }
}
