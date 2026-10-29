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

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.CreateComprobanteVenta;

public sealed class CreateComprobanteVentaCommandValidator : AbstractValidator<CreateComprobanteVentaCommand>
{
    public CreateComprobanteVentaCommandValidator()
    {
        RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Number).MaximumLength(10);
        RuleFor(x => x.PlantCode).MaximumLength(2);
        RuleFor(x => x.ExecutingPlantCode).MaximumLength(2);
        RuleFor(x => x.Glosa).MaximumLength(200);
        RuleFor(x => x.Guias).NotEmpty().WithMessage("Indique al menos una guía.");
        RuleForEach(x => x.Guias).NotEmpty().MaximumLength(10);
    }
}
