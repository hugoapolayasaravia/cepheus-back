using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.CreateGuiaRemisionVenta;

public sealed class GuiaRemisionVentaFieldsValidator<T> : AbstractValidator<T> where T : GuiaRemisionVentaFields
{
    public GuiaRemisionVentaFieldsValidator()
    {
        RuleFor(x => x.TransportistaCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.ChoferCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.VehicleCode).MaximumLength(4);
        RuleFor(x => x.Address).MaximumLength(200);
        RuleFor(x => x.UbigeoCode).MaximumLength(6);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(250);
        RuleFor(x => x.FleteCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.FleteAmount).GreaterThanOrEqualTo(0m).When(x => x.FleteAmount.HasValue);
        RuleFor(x => x.DispatchTime).MaximumLength(5);
        RuleFor(x => x.TicketNumber).MaximumLength(20);
        RuleFor(x => x.PumpCode).MaximumLength(4);
    }
}

public sealed class GuiaRemisionVentaDetalleRequestValidator : AbstractValidator<GuiaRemisionVentaDetalleRequest>
{
    public GuiaRemisionVentaDetalleRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0m);
        RuleFor(x => x.Item).GreaterThan(0).When(x => x.Item.HasValue);
        RuleFor(x => x.ProductoTipoCode).MaximumLength(2);
        RuleFor(x => x.ProductoCode).MaximumLength(4);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0m).When(x => x.UnitPrice.HasValue);
    }
}

public sealed class CreateGuiaRemisionVentaCommandValidator : AbstractValidator<CreateGuiaRemisionVentaCommand>
{
    public CreateGuiaRemisionVentaCommandValidator()
    {
        Include(new GuiaRemisionVentaFieldsValidator<CreateGuiaRemisionVentaCommand>());

        RuleFor(x => x.Number).MaximumLength(10);
        RuleFor(x => x.OrdenTrabajoCode).MaximumLength(8);
        RuleFor(x => x.ProgramacionCode).MaximumLength(8);
        RuleFor(x => x.PlantCode).MaximumLength(2);
        RuleFor(x => x.ClienteCode).MaximumLength(5);
        RuleFor(x => x.ObraCode).MaximumLength(3);
        RuleFor(x => x.VendedorCode).MaximumLength(4);
        RuleFor(x => x.ReplacesNumber).MaximumLength(10);
        RuleFor(x => x.DevolutionMotiveCode).MaximumLength(2);
        RuleFor(x => x.DevolutionActionCode).MaximumLength(2);

        RuleFor(x => x.Detalles).NotEmpty().WithMessage("La guía debe tener al menos una línea.");
        RuleForEach(x => x.Detalles).SetValidator(new GuiaRemisionVentaDetalleRequestValidator());
    }
}
